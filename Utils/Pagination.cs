using ChatAppApi.Dtos;
using ChatAppApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace ChatAppApi.Utils
{
    public class Pagination<T>
    {
        public async static Task<Page<T>> Execute(IQueryable<T> query, Pageable pageable)
        {
            string sortBy = pageable.Sort + " " + pageable.Direction;
            query = query.OrderBy(sortBy);
            int totalItems = await query.CountAsync();

            List<T> contents = await query
                .Skip(pageable.Page * pageable.Size)
                .Take(pageable.Size)
                .ToListAsync();

            Page<T> result = new Page<T>
            {
                Contents = contents,
                PageNumber = pageable.Page,
                PageSize = pageable.Size,
                TotalItems = totalItems
            };
            return result;
        }
    }
}
