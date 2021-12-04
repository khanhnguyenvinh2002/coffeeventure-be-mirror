using coffeeventureAPI.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coffeeventureAPI.Model.Base
{
    public static class BaseExtension
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> source, BaseRequestDto requestPayload) {
            int size = 10;
            int index = 0;
            if(requestPayload.PageSize != null)
            {
                size = (int)requestPayload.PageSize;
            }

            if (requestPayload.PageIndex != null)
            {
                index = (int)requestPayload.PageIndex;
            }
            source = source.Skip((index) * size).Take(size);
            return source;
        }
    }
}
