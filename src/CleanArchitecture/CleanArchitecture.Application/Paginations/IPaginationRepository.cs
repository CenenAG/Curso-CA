

using System.Linq.Expressions;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Users;
using Microsoft.EntityFrameworkCore.Query;

namespace CleanArchitecture.Application.Paginations;
public interface IPaginationRepository<TEntity, TEntityId>
where TEntity : Entity<TEntityId>
where TEntityId : class
{
    Task<PagedResults<TEntity, TEntityId>> GetPaginationAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
        int pageNumber,
        int pageSize,
        string orderBy,
        bool isAscending,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);
}
