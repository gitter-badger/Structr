using Microsoft.EntityFrameworkCore;
using Structr.Abstractions;
using Structr.Collections;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Structr.EntityFrameworkCore
{
    public static class QueryableExtensions
    {
        public static IPagedList<TSource> ToPagedList<TSource>(this IQueryable<TSource> source, int pageSize, int pageNumber)
        {
            Ensure.NotNull(source, nameof(source));

            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "Page number must be greater or equal 1");

            var totalItems = source.Count();
            if (totalItems == 0)
                return PagedList.Empty<TSource>();

            if (pageSize > 0)
            {
                var skip = (pageNumber - 1) * pageSize;
                source = source.Skip(skip).Take(pageSize);
            }

            return new PagedList<TSource>(source.ToList(), totalItems, pageSize > 0 ? pageSize : totalItems, pageNumber);
        }

        public static async Task<IPagedList<TSource>> ToPagedListAsync<TSource>(this IQueryable<TSource> source,
            int pageSize, int pageNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.NotNull(source, nameof(source));

            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "Page number must be greater or equal 1");

            var totalItems = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            if (totalItems == 0)
                return PagedList.Empty<TSource>();

            if (pageSize > 0)
            {
                var skip = (pageNumber - 1) * pageSize;
                source = source.Skip(skip).Take(pageSize);
            }

            return new PagedList<TSource>(await source.ToListAsync(cancellationToken).ConfigureAwait(false), totalItems, pageSize > 0 ? pageSize : totalItems, pageNumber);
        }
    }
}
