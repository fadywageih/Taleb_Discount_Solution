using System.Linq.Expressions;

namespace Domain.Contracts
{
    public abstract class Specifications<T> where T : class
    {
        protected Specifications(Expression<Func<T, bool>>? criteria)
        {
            Criteria = criteria;
        }
        public Expression<Func<T, bool>>? Criteria { get; }
        public List<Expression<Func<T, object>>> IncludeExpression { get; } = new();
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPaginated { get; private set; }
        protected void AddInclude(Expression<Func<T, object>> Expression) => IncludeExpression.Add(Expression);
        protected void SetOrderBy(Expression<Func<T, object>> Expression) => OrderBy = Expression;
        protected void SetOrderByDescending(Expression<Func<T, object>> Expression) => OrderByDescending = Expression;
        protected void ApplyPagination(int PageIndex, int PageSize)
        {
            Take = PageSize;
            Skip = (PageIndex - 1) * PageSize;
            IsPaginated = true;
        }
    }
}
