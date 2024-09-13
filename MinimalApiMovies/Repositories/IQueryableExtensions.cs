using MinimalApiMovies.DTOs;

namespace MinimalApiMovies.Repositories {
    public static class IQueryableExtensions {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO pagination) {
            return queryable
                .Skip(( pagination.Page - 1 ) * pagination.RecorsPerPage)
                .Take(pagination.RecorsPerPage);
        }
    }
}
