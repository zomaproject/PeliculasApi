﻿using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace PeliculasApi.Helpers;

public static class HttpContextExtensions
{
    public static async Task InsertarParametrosPaginacion<T>(this HttpContext httpContext,
        IQueryable<T> queryable, int cantidadRegistrosPorPagina)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        double cantidad = await queryable.CountAsync();
        double cantidadPaginas = Math.Ceiling(cantidad / cantidadRegistrosPorPagina);
        httpContext.Response.Headers.Add("cantidadPaginas", cantidadPaginas.ToString(CultureInfo.InvariantCulture));
    }
}