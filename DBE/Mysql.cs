using MySql.Data.MySqlClient;
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
        #region Members
        private MySqlConnection? conn;
        #endregion //Members

        #region Events
        public event IDatabaseConnection.OnExceptionHandler? OnException;
        #endregion //Events

        #region Implementations
        public bool Open(IPAddress ip, int port, string database, string username, string password)
        {
            string connectionString = "SERVER=" + ip.ToString() + "; PORT=" + port + ";" + "DATABASE=" + database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
            this.conn = new MySqlConnection(connectionString);

            try
            {
                this.conn.Open();
                return true;
            }
            catch(MySqlException mex)
            {
                this.OnException?.Invoke(mex);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                this.conn?.Close();
                return true;
            }
            catch(MySqlException mex)
            {
                this.OnException?.Invoke(mex);
                return false;
            }
        }

        public object[]? Execute(Query query)
        {
            string q = this.Compose(query);

            return null;
        }

        public string Compose(Query query)
        {
            List<string> queryBuffer = [];
            if(query.ISelect.Length != 0)
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
            if(query.IInnerJoin.Length != 0)
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
                                    field = field[6..];
                                }
                                else if(field.StartsWith("Object_"))
                                {
                                    field = string.Concat(field.AsSpan(7), "Id");
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

                            if (value is ITable table)
                            {
                                if (table is ITable<int> iInt)
                                {
                                    int? primary = iInt.GetPrimary();
                                    return primary == null ? "NULL" : primary.ToString() ?? "NULL";
                                }
                            }

                            return value.ToString() ?? "NULL";
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
