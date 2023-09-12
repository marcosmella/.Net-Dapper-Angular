using System;
using System.Collections.Generic;
using System.Linq;

namespace VL.Health.Domain.Entities
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int? pageSize)
        {
            var count = source.Count();
            int rowsPerPage = pageSize.HasValue ? (int)pageSize : count;
            var items = source.Skip((pageNumber - 1) * rowsPerPage).Take(rowsPerPage).ToList();
            return new PagedList<T>(items, count, pageNumber, rowsPerPage);
        }
    }
}
