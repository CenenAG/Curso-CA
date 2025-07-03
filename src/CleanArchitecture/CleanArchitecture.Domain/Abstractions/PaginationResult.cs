using System.Collections.Generic;

namespace CleanArchitecture.Domain.Abstractions
{
    public class PaginationResult<TEntity, TEntityId>
    {
        public int Count { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IReadOnlyList<TEntity>? Data { get; set; }
        public int PagesCount { get; }
        public int ResultByPage { get; set; }

    }
}
