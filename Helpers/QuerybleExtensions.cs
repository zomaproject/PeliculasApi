using PeliculasApi.DTOS;

namespace PeliculasApi.Helpers;

public static class QuerybleExtensions
{
   public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDto paginacionDto)
   {
      return queryable
         .Skip((paginacionDto.Pagina - 1) * paginacionDto.CantidadRegistrosPorPagina)
         .Take(paginacionDto.CantidadRegistrosPorPagina);
   }
}