using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Common
{
    public class PaginatedList<TEntity> : IEnumerable<TEntity>
    {
        private readonly List<TEntity> _list;

        public PaginatedList(IEnumerable<TEntity> items, int totalCount, int pageIndex, int pageSize)
        {
            _list = new List<TEntity>();
            _list.AddRange(items);
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            TotalCount = totalCount;
        }

        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;
        public IReadOnlyList<TEntity> Items => _list;

        /// <inheritdoc />
        public IEnumerator<TEntity> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}