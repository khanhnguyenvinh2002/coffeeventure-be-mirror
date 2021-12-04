using coffeeventureAPI.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coffeeventureAPI.Core.Repository
{
    public class BaseRepository
    {
        protected BaseRepository()
        {
        }
    }

    public static class BaseRepositoryExtension
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> source, BaseRequestDto requestPayload)
        {
            if (requestPayload.PageSize.HasValue && requestPayload.PageSize.Value > 0)
            {
                int skip = requestPayload.PageSize.Value * (requestPayload.PageIndex ?? 0);
                source = source.Skip(skip).Take(requestPayload.PageSize.Value);
            }

            return source;
        }
    }

    public static class LinqExtension
    {
        public static bool ContainsAny(this string haystack, params string[] needles)
        {
            foreach (string needle in needles)
            {
                if (haystack.Contains(needle))
                    return true;
            }

            return false;
        }

        public static bool EqualsAny(this string haystack, params string[] needles)
        {
            foreach (string needle in needles)
            {
                if (haystack == needle)
                    return true;
            }

            return false;
        }

        public static bool ContainsRemoveDiacritics(this string target, string filter)
        {
            var text = filter.Split("&").Select(x => x.ToLower().RemoveDiacritics());
            if (target.ToLower().RemoveDiacritics().ContainsAny(text.ToArray()))
            {
                return true;
            }

            return false;
        }

        static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
