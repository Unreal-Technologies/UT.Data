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
            this.select = new List<LambdaExpression>();
            this.join = new List<Tuple<Joins, LambdaExpression>>();
        }
        #endregion //Constructors

        #region Public Methods
        public void Select<Ttable, Treturn>(Expression<Func<Ttable, Treturn>> fnc)
            where Ttable : ITable
            where Treturn : class
        {
            this.select.Add(fnc);
        }

        public void From<T>()
        {
            this.from = typeof(T);
        }

        public void Where<T>(Expression<Func<T, bool>> fnc)
            where T : ITable
        {
            this.where = fnc;
        }

        public void InnerJoin<Tleft, Tright>(Expression<Func<Tleft, Tright, bool>> fnc)
            where Tleft : ITable
            where Tright : ITable
        {
            this.join.Add(new Tuple<Joins, LambdaExpression>(Joins.InnerJoin, fnc));
        }

        public void LeftOuterJoin<Tleft, Tright>(Expression<Func<Tleft, Tright, bool>> fnc)
            where Tleft : ITable
            where Tright : ITable
        {
            this.join.Add(new Tuple<Joins, LambdaExpression>(Joins.LeftOuterJoin, fnc));
        }

        public void RightOuterJoin<Tleft, Tright>(Expression<Func<Tleft, Tright, bool>> fnc)
            where Tleft : ITable
            where Tright : ITable
        {
            this.join.Add(new Tuple<Joins, LambdaExpression>(Joins.RightOuterJoin, fnc));
        }

        public override string ToString()
        {
            return this.queryable.Compose(this);
        }
        #endregion //Public Methods
    }
}
