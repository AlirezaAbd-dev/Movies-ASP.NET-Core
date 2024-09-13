﻿using Microsoft.EntityFrameworkCore;

namespace MinimalApiMovies.Repositories {
    public static class HttpContextExtensions {
        public async static Task InsertPaginationParameterInResponseHeader<T>(
            this HttpContext httpContext, IQueryable<T> queryable
            ) {
            if( httpContext == null )
                throw new ArgumentNullException(nameof(httpContext));

            double count = await queryable.CountAsync();
            httpContext.Response.Headers.Append("total-Amount-Of-Records", count.ToString());

        }
    }
}
