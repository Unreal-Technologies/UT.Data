using System.Linq.Expressions;
using System.Reflection;

namespace UT.Data.DBE
{
    public class Mysql : IQueryable
    {
        #region Implementations
        string IQueryable.Compose(Query query)
        {
            List<string> queryBuffer = new();
            if(query.ISelect.Count() != 0)
            {
                queryBuffer.Add("select");
                List<string> select = new();
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
        private string Parse(Expression expression)
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
                        string table = member.Expression?.Type.Name ?? string.Empty;
                        string field = member.Member.Name;

                        return "`" + table + "`.`" + field + "`";
                    }
                    break;
                case ExpressionType.Equal:
                    if(expression is BinaryExpression binary)
                    {
                        string left = this.Parse(binary.Left);
                        string right = this.Parse(binary.Right);
                        return left + " = " + right;
                    }
                    break;
                case ExpressionType.Constant:
                    if(expression is ConstantExpression constant)
                    {
                        return constant.ToString();
                    }
                    break;
                case ExpressionType.Parameter:
                    if(expression is ParameterExpression parameter)
                    {
                        return "`" + parameter.Type.Name + "`";
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
