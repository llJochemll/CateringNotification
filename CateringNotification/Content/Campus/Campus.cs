using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CateringNotification.Content.Campus
{
    internal static class Campus
    {
        internal static async Task<string> GetMenuAsync(string url)
        {
            return
                "<div style=\" text-align: center;\">" +
                "<h1  style=\"color: #58A518;\">Het menu van vandaag is:</h1><hr>" +
                "<p style=\"font-family: impact; font-size: 18px;\">" +
                string.Join("</p><p style=\"font-family: impact; font-size: 18px;\">", await GetMenuItemsAsync(url)) +
                "</p></div>";
        }

        private static async Task<IEnumerable<string>> GetMenuItemsAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var pageContent = await httpClient.GetStringAsync(url);

                if (pageContent == null)
                {
                    return new List<string>();
                }

                var beginIndex = pageContent.IndexOf("<h3>Hoofdgerecht</h3>", StringComparison.Ordinal) + "<h3>Hoofdgerecht</h3>".Length;
                var endIndex = pageContent.IndexOf("<p><strong>", beginIndex, StringComparison.Ordinal);

                if (beginIndex >= endIndex)
                {
                    return new List<string>();
                }

                return pageContent.Substring(beginIndex, endIndex - beginIndex).Split("<p>")
                    .AsSpan()
                    .Slice(1)
                    .ToArray()
                    .Select(e => e.Replace("</p>", ""));
            }
        }
    }
}
