using System.Linq.Expressions;
using UT.Data.Attributes;

namespace UT.Data.DBE
{
    public class Query
    {
        #region Enums
        public enum Joins
        {
            [Description("inner join")]
            InnerJoin, 
            [Description("left outer join")]
            LeftOuterJoin, 
            [Description("right outer join")]
            RightOuterJoin
        }
        #endregion //Enums

        #region Members
        private readonly IQueryable queryable;
        private readonly List<LambdaExpression> select;
        private readonly List<Tuple<Joins, LambdaExpression>> join;
        private Type? from;
        private LambdaExpression? where;
        #endregion //Members

        #region Properties
        internal Tuple<Joins, LambdaExpression>[] IInnerJoin
        {
            get { return this.join.ToArray(); }
        }

        internal LambdaExpression[] ISelect
        {
            get { return this.select.ToArray(); }
        }

        internal Type? IFrom
        {
            get { return this.from; }
        }

        internal LambdaExpression? IWhere
        {
            get { return this.where; }
        }
        #endregion //Properties

        #region Constructors
        public Query(IQueryable queryable)
        {
            this.queryable = queryable;
            this.select = [];
            this.join = [];
        }
        #endregion //Constructors

        #region Public Methods
        public object[]? Execute()
        {
            return this.queryable.Execute(this);
        }

        public Query Select<Ttable, Treturn>(Expression<Func<Ttable, Treturn>> fnc)
            where Ttable : ITable
        {
            this.select.Add(fnc);
            return this;
        }

        public Query From<T>()
        {
            this.from = typeof(T);
            return this;
        }

        public Query Where<T>(Expression<Func<T, bool>> fnc)
            where T : ITable
        {
            this.where = fnc;
            return this;
        }

        public Query InnerJoin<Tleft, Tright>(Expression<Func<Tleft, Tright, bool>> fnc)
            where Tleft : ITable
            where Tright : ITable
        {
            this.join.Add(new Tuple<Joins, LambdaExpression>(Joins.InnerJoin, fnc));
            return this;
        }

        public Query LeftOuterJoin<Tleft, Tright>(Expression<Func<Tleft, Tright, bool>> fnc)
            where Tleft : ITable
            where Tright : ITable
        {
            this.join.Add(new Tuple<Joins, LambdaExpression>(Joins.LeftOuterJoin, fnc));
            return this;
        }

        public Query RightOuterJoin<Tleft, Tright>(Expression<Func<Tleft, Tright, bool>> fnc)
            where Tleft : ITable
            where Tright : ITable
        {
            this.join.Add(new Tuple<Joins, LambdaExpression>(Joins.RightOuterJoin, fnc));
            return this;
        }

        public override string ToString()
        {
            return this.queryable.Compose(this);
        }
        #endregion //Public Methods
    }
}
