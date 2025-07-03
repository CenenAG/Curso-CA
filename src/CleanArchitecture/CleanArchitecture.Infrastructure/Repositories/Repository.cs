using System.Linq.Expressions;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Infrastructure.Extensions;
using CleanArchitecture.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace CleanArchitecture.Infrastructure.Repositories;

internal abstract class Repository<TEntity, TEntityId>
where TEntity : Entity<TEntityId>
where TEntityId : class
{
    protected readonly ApplicationDbContext _dbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TEntity?> GetByIdAsync(TEntityId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public void Add(TEntity entity) => _dbContext.Add(entity);


    public IQueryable<TEntity> ApplySpecification(ISpecification<TEntity, TEntityId> spec)
    {
        return SpecificationEvaluator<TEntity, TEntityId>
        .GetQuery(_dbContext.Set<TEntity>().AsQueryable(), spec);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllWithSpecAsync(
        ISpecification<TEntity, TEntityId> spec,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity, TEntityId> spec,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).CountAsync(cancellationToken);
    }

    public async Task<PagedResults<TEntity, TEntityId>> GetPaginationAsync(
        Expression<Func<TEntity, bool>> predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
        int pageNumber,
        int pageSize,
        string orderBy,
        bool isAscending,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = _dbContext.Set<TEntity>();

        if (disableTracking) queryable = queryable.AsNoTracking();


        if (predicate != null) queryable = queryable.Where(predicate);

        if (includes != null) queryable = includes(queryable);

        var skipAmount = pageSize * (pageNumber - 1);
        var totalNumberOfRecords = await queryable.CountAsync(cancellationToken);
        var records = new List<TEntity>();

        if (string.IsNullOrEmpty(orderBy))
        {
            records = await queryable.Skip(skipAmount).Take(pageSize).ToListAsync(cancellationToken);
        }
        else
        {
            records = await queryable
            .OrderByPropertyOrField(orderBy, isAscending)
            .Skip(skipAmount)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        }

        var mod = totalNumberOfRecords % pageSize;
        var totalPageCount = (totalNumberOfRecords / pageSize) + (mod > 0 ? 1 : 0);

        return new PagedResults<TEntity, TEntityId>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalNumberOfPages = totalPageCount,
            TotalNumberOfRecords = totalNumberOfRecords,
            Results = records
        };
    }


}