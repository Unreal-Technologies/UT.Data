using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using UT.Data.Attributes;

namespace UT.Data.DBE
{
    [Default("Server", "127.0.0.1")]
    [Default("Port", "3306")]
    [Default("Username", "root")]
    [Default("Password", "")]
    public class Mysql : IQueryable, IDatabaseConnection
    {
        #region Implementations
        public bool Connect(IPAddress ip, int port, string database, string username, string password)
        {
            return true;
        }

        public bool Close()
        {
            return false;
        }

        public object[]? Execute(Query query)
        {
            string q = this.Compose(query);

            return null;
        }

        public string Compose(Query query)
        {
            List<string> queryBuffer = [];
            if(query.ISelect.Count() != 0)
            {
                queryBuffer.Add("select");
                List<string> select = [];
                foreach(LambdaExpression expression in query.ISelect)
                {
                    select.Add(this.Parse(expression));
                }
                queryBuffer.Add(string.Join(", ", select));
            }
            if(query.IFrom != null)
            {
                queryBuffer.Add("from `" + query.IFrom.Name + "`");
            }
            if(query.IInnerJoin.Count() != 0)
            {
                foreach (Tuple<Query.Joins, LambdaExpression> tuple in query.IInnerJoin)
                {
                    string param = this.Parse(tuple.Item2.Parameters[1]);
                    queryBuffer.Add(tuple.Item1.ToString() + " " + param + " on " + this.Parse(tuple.Item2));
                }
            }
            if(query.IWhere != null)
            {
                queryBuffer.Add("where " + this.Parse(query.IWhere));
            }

            return string.Join("\r\n", queryBuffer);
        }
        #endregion //Implementations

        #region Private Methods
        private string Parse(Expression expression, string? param = null)
        {
            switch(expression.NodeType)
            {
                case ExpressionType.Lambda:
                    if (expression is LambdaExpression lambda)
                    {
                        return this.Parse(lambda.Body);
                    }
                    break;
                case ExpressionType.MemberAccess:
                    if(expression is MemberExpression member)
                    {
                        ExpressionType? type = member.Expression?.NodeType;
                        if(type == null)
                        {
                            break;
                        }
                        switch(type)
                        {
                            case ExpressionType.Parameter:
                                string table = member.Expression?.Type.Name ?? string.Empty;
                                DescriptionAttribute? description = member.Expression?.Type.GetCustomAttribute<DescriptionAttribute>();
                                if(description != null)
                                {
                                    table = description.Text;
                                }

                                string field = member.Member.Name;
                                if(field.StartsWith("Field_"))
                                {
                                    field = field.Substring(6);
                                }
                                else if(field.StartsWith("Object_"))
                                {
                                    field = field.Substring(7) + "Id";
                                }

                                return "`" + table + "`.`" + field + "`";
                            case ExpressionType.Constant:
                                if(member.Expression == null)
                                {
                                    return "";
                                }
                                return "'" + this.Parse(member.Expression, member.Member.Name) + "'";
                            default:
                                throw new NotImplementedException(type.ToString());
                        }
                    }
                    break;
                case ExpressionType.Equal:
                    if(expression is BinaryExpression binaryEqual)
                    {
                        string left = this.Parse(binaryEqual.Left);
                        string right = this.Parse(binaryEqual.Right);
                        return left + " = " + right;
                    }
                    break;
                case ExpressionType.Constant:
                    if(expression is ConstantExpression constant)
                    {
                        if (constant.Type == typeof(string) || constant.Type == typeof(int))
                        {
                            return constant.ToString();
                        }
                        else
                        {
                            object? obj = constant.Value;
                            if(obj == null || param == null)
                            {
                                return "";
                            }
                            Type type = obj.GetType();
                            object? value = type.GetRuntimeField(param)?.GetValue(obj);
                            if(value == null)
                            {
                                return "";
                            }

                            ITable? table = value as ITable;
                            if(table != null)
                            {
                                ITable<int>? iInt = table as ITable<int>;
                                if(iInt != null)
                                {
                                    int? primary = iInt.GetPrimary();
                                    return primary == null ? "NULL" : primary.ToString();
                                }
                            }

                            return value.ToString();
                        }
                    }
                    break;
                case ExpressionType.Parameter:
                    if(expression is ParameterExpression parameter)
                    {
                        return "`" + parameter.Type.Name + "`";
                    }
                    break;
                case ExpressionType.AndAlso:
                    if (expression is BinaryExpression binaryAndAlso)
                    {
                        string left = this.Parse(binaryAndAlso.Left);
                        string right = this.Parse(binaryAndAlso.Right);
                        return "("+left + " and " + right+")";
                    }

                    break;
                default:
                    throw new NotImplementedException(expression.NodeType.ToString());
            }
            return String.Empty;
        }
        #endregion //Private Methods
    }
}
