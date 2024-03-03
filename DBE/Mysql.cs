using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using UT.Data.Attributes;
using UT.Data.DBE.Attributes;
using UT.Data.Extensions;

namespace UT.Data.DBE
{
    [Data.Attributes.Default("Server", "127.0.0.1")]
    [Data.Attributes.Default("Port", "3306")]
    [Data.Attributes.Default("Username", "root")]
    [Data.Attributes.Default("Password", "")]
    public class Mysql : IQueryable, IDatabaseConnection
    {
        #region Members
        private MySqlConnection? conn;
        #endregion //Members

        #region Events
        public event IDatabaseConnection.OnExceptionHandler? OnException;
        #endregion //Events

        #region Implementations
        public void CreateOrUpdateTable(ITable table)
        {
            DescriptionAttribute? description = table.GetType().GetCustomAttribute<DescriptionAttribute>();
            string tableName = description == null ? table.GetType().Name : description.Text;

            Resultset rs = this.Execute("show tables like '" + tableName + "'");

            if (!(table.GetType() == typeof(TableInfo)))
            {
                TableInfo.CreateOrUpdate(this);
            }

            if(rs.Rows == 0)
            {
                this.CreateOrUpdateTable_Create(table);
            }
            else
            {
                this.CreateOrUpdateTable_Update(table);
            }
        }

        public bool Save<T>(ITable<T> table, bool refresh = false)
            where T : struct
        {
            Dictionary<string, string> values = [];
            foreach (FieldInfo fi in table.GetType().GetRuntimeFields())
            {
                PrimaryKeyAttribute? pka = fi.GetCustomAttribute<PrimaryKeyAttribute>();
                if (pka == null && table.Changed.Contains(fi.Name))
                {
                    values.Add(fi.Name, GetMysqlValue(fi.GetValue(table)));
                }
            }

            DescriptionAttribute? description = table.GetAttribute<DescriptionAttribute>();
            string tableName = description != null ? description.Text : table.GetType().Name;

            T? primary = table.GetPrimary();
            if (primary == null)
            {
                List<string> ks = [];
                List<string> vs = [];
                foreach(KeyValuePair<string, string> kvp in values)
                {
                    ks.Add(kvp.Key);
                    vs.Add(kvp.Value);
                }

                string composed = "insert into `" + tableName + "`(`" + string.Join("`, `", ks) + "`) values(" + string.Join(", ", vs) + ")";
                Resultset rs = this.Execute(composed);
                if(refresh && rs.IsSuccess)
                {
                    throw new NotImplementedException("SAVE - REFRESH");
                }

                return rs.IsSuccess;
            }
            else
            {
                throw new NotImplementedException("UPDATE");
            }
        }

        public bool StartTransaction()
        {
            return this.Execute("start transaction").IsSuccess;
        }

        public bool CommitTransaction()
        {
            return this.Execute("commit").IsSuccess;
        }

        public bool RevertTransaction()
        {
            return this.Execute("rollback").IsSuccess;
        }

        private void CreateOrUpdateTable_Update(ITable table)
        {
            string hash = CalculateHash(table);

            DescriptionAttribute? description = table.GetAttribute<DescriptionAttribute>();
            string tableName = description != null ? description.Text : table.GetType().Name;

            Resultset rs = this.Execute("select `Id` from `ut.data-tableInfo` where `table` = '" + tableName + "' and `hash` != '" + hash + "'");
            if(rs.IsSuccess && rs.Rows > 0)
            {
                throw new NotImplementedException("UPDATE TABLE");
            }
        }

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
            Resultset rs = this.Execute(this.Compose(query));

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
                    select.Add(Parse(expression));
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
                    string param = Parse(tuple.Item2.Parameters[1]);
                    queryBuffer.Add(tuple.Item1.ToString() + " " + param + " on " + Parse(tuple.Item2));
                }
            }
            if(query.IWhere != null)
            {
                queryBuffer.Add("where " + Parse(query.IWhere));
            }

            return string.Join("\r\n", queryBuffer);
        }
        #endregion //Implementations

        #region Private Methods
        private bool CreateOrUpdateTable_Create(ITable table)
        {
            string hash = CalculateHash(table);
            List<string> buffer = [];
            string? primary = null;

            foreach (FieldInfo fi in table.GetType().GetRuntimeFields())
            {
                PrimaryKeyAttribute? pka = fi.GetCustomAttribute<PrimaryKeyAttribute>();
                if (pka != null)
                {
                    primary = fi.Name;
                }

                buffer.Add("`" + fi.Name + "` " + GetMysqlType(fi) + (pka != null ? " auto_increment" : ""));

            }
            if (primary != null)
            {
                buffer.Add("primary key(`" + primary + "`)");
            }

            DescriptionAttribute? description = table.GetAttribute<DescriptionAttribute>();
            string tableName = description != null ? description.Text : table.GetType().Name;

            if (this.StartTransaction())
            {
                string composed = "create table `" + tableName + "` (" + string.Join(", ", buffer) + ") engine=InnoDB";
                if (!this.Execute(composed).IsSuccess)
                {
                    this.RevertTransaction();
                    return false;
                }

                TableInfo ti = new()
                {
                    Field_Hash = hash,
                    Field_Table = tableName
                };
                if (!this.Save(ti))
                {
                    this.RevertTransaction();
                    return false;
                }

                return this.CommitTransaction();
            }
            return false;
        }

        private static string GetMysqlValue(object? value)
        {
            if(value == null)
            {
                return "NULL";
            }
            else if(value.GetType() == typeof(int))
            {
                return value?.ToString() ?? "0";
            }
            else if(value.GetType() == typeof(string))
            {
                return "'" + value.ToString() + "'";
            }
            else if(value.GetType() == typeof(DateTime))
            {
                DateTime? dt = (DateTime?)value;
                if(dt == null || dt == DateTime.MinValue)
                {
                    return "NULL";
                }

                return "'" + dt.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }

            throw new NotImplementedException();
        }

        private static string GetMysqlType(FieldInfo fi)
        {
            Type? t;
            try
            {
                t = Type.GetType(fi.FieldType.ToString().Split('[')[1].Split(']')[0]);
            }
            catch (Exception)
            {
                t = fi.FieldType;
            }

            LengthAttribute? lengthAttribute = fi.GetCustomAttribute<LengthAttribute>();
            Attributes.DefaultAttribute? defaultAttribute = fi.GetCustomAttribute<Attributes.DefaultAttribute>();

            bool nullable = false;
            string? defaultValue = defaultAttribute?.Value;
            int? length = lengthAttribute?.Value;

            string partialComposed = (nullable ? "" : " not") + " null" + (defaultValue != null ? " default " + defaultValue : "");

            if (t == typeof(int))
            {
                return "int(" + (length == null ? 10 : length) + ")" + partialComposed;
            }
            else if (t == typeof(string) && length == null)
            {
                return "longtext" + partialComposed;
            }
            else if (t == typeof(string) && length != null)
            {
                return "varchar(" + length + ")" + partialComposed;
            }
            else if(t == typeof(DateTime))
            {
                return "datetime" + partialComposed;
            }
            else
            {
                throw new NotImplementedException(t?.AssemblyQualifiedName ?? "");
            }
        }

        private static string CalculateHash(ITable table)
        {
            List<string> buffer = [];
            foreach (FieldInfo fi in table.GetType().GetRuntimeFields())
            {
                PrimaryKeyAttribute? pka = fi.GetCustomAttribute<PrimaryKeyAttribute>();
                int hasPrimaryKey = pka != null ? 1 : 0;


                string composed = fi.Name + ":" + hasPrimaryKey;
                buffer.Add(composed);
            }

            return string.Join('|', buffer).Md5();
        }

        private Resultset Execute(string query)
        {
            bool isSelect = query.StartsWith("select", StringComparison.OrdinalIgnoreCase);
            bool isDescribe = query.StartsWith("describe", StringComparison.OrdinalIgnoreCase);
            bool isShow = query.StartsWith("show", StringComparison.OrdinalIgnoreCase);
            try
            {
                if (isSelect || isDescribe || isShow)
                {
                    MySqlCommand cmd = new(query, this.conn);
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    int rows = 0;
                    int cols = 0;
                    List<Dictionary<string, object>> data = [];
                    while (dataReader.Read())
                    {
                        Dictionary<string, object> rw = [];
                        if (rows == 0)
                        {
                            cols = dataReader.FieldCount;
                        }

                        for (int i = 0; i < cols; i++)
                        {
                            rw.Add(dataReader.GetName(i), dataReader[i]);
                        }
                        data.Add(rw);

                        rows++;
                    }
                    dataReader.Close();

                    return new Resultset()
                    {
                        IsSuccess = true,
                        Rows = rows,
                        Cols = cols,
                        Data = data.ToArray(),
                        Query = query
                    };
                }
                else
                {
                    MySqlCommand cmd = new MySqlCommand(query, this.conn);
                    cmd.ExecuteNonQuery();

                    return new Resultset()
                    {
                        IsSuccess = true,
                        Query = query,
                        LastInsertId = cmd.LastInsertedId
                    };
                }
            }
            catch (MySqlException mex)
            {
                this.OnException?.Invoke(mex);

                return new Resultset()
                {
                    IsSuccess = false,
                    Query = query,
                    Exception = mex
                };
            }
        }

        private static string Parse(Expression expression, string? param = null)
        {
            switch(expression.NodeType)
            {
                case ExpressionType.Lambda:
                    if (expression is LambdaExpression lambda)
                    {
                        return Parse(lambda.Body);
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
                                return "'" + Parse(member.Expression, member.Member.Name) + "'";
                            default:
                                throw new NotImplementedException(type.ToString());
                        }
                    }
                    break;
                case ExpressionType.Equal:
                    if(expression is BinaryExpression binaryEqual)
                    {
                        string left = Parse(binaryEqual.Left);
                        string right = Parse(binaryEqual.Right);
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
                        string left = Parse(binaryAndAlso.Left);
                        string right = Parse(binaryAndAlso.Right);
                        return "("+left + " and " + right+")";
                    }

                    break;
                default:
                    throw new NotImplementedException(expression.NodeType.ToString());
            }
            return String.Empty;
        }
        #endregion //Private Methods

        #region Classes
        public class Resultset
        {
            #region Members
            private int position = 0;
            #endregion //Members

            #region Properties
            public string Query { get; set; }
            public bool IsSuccess { get; set; }
            public int Rows { get; set; }
            public int Cols { get; set; }
            public Dictionary<string, object>[] Data { get; set; }
            public long LastInsertId { get; set; }
            public Exception Exception { get; set; }
            #endregion //Properties

            #region Public Methods
            public void Reset()
            {
                this.position = 0;
            }

            //public T Last<T>()
            //    where T : Table<T>
            //{
            //    return this.ConvertToObject<T>(this.Last());
            //}

            public Dictionary<string, object>? Last()
            {
                return this.Rows == 0 ? null : this.Data[this.Data.Length - 1];
            }

            //public T First<T>()
            //    where T : Table<T>
            //{
            //    return this.ConvertToObject<T>(this.First());
            //}

            public Dictionary<string, object>? First()
            {
                return this.Rows == 0 ? null : this.Data[0];
            }

            //public T Read<T>()
            //    where T : Table<T>
            //{
            //    return this.ConvertToObject<T>(this.Read());
            //}

            public Dictionary<string, object>? Read()
            {
                if (this.Data == null || this.position >= this.Data.Length)
                {
                    return null;
                }
                return this.Data[this.position++];
            }
            #endregion //Public Methods

            #region Private Methods
            //private T ConvertToObject<T>(Dictionary<string, object> values)
            //    where T : Table<T>
            //{
            //    if (values == null)
            //    {
            //        return null;
            //    }

            //    Type t = typeof(T);
            //    T instance = (T)Activator.CreateInstance<T>();
            //    foreach (KeyValuePair<string, object> kvp in values)
            //    {
            //        PropertyInfo pi = t.GetRuntimeProperty(kvp.Key);
            //        if (pi != null)
            //        {
            //            pi.SetValue(instance, this.MysqlTypeConversion(pi.PropertyType, kvp.Value));
            //        }
            //    }

            //    return instance;
            //}

            //private object? MysqlTypeConversion(Type target, object value)
            //{
            //    Type t = value.GetType();
            //    if (target == t)
            //    {
            //        return value;
            //    }
            //    if (target == typeof(bool?))
            //    {
            //        if (value != null)
            //        {
            //            return value.ToString() == "True";
            //        }

            //        return null;
            //    }
            //    if (target == typeof(int?))
            //    {
            //        if (value != null)
            //        {
            //            return int.Parse(value.ToString());
            //        }

            //        return null;
            //    }

            //    throw new NotImplementedException("Undefined type `" + t.FullName + "`");
            //}
            #endregion //Private Methods
        }
        #endregion //Classes
    }
}
