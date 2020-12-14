using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CateringNotification.Content.Campus
{
    public static class Campus
    {
        public static async Task<string> GetMenuAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            using (var httpClient = new HttpClient())
            {
                var pageContent = await httpClient.GetStringAsync(url);

                if (pageContent == null)
                {
                    return null;
                }



                const string beginTag = "<div class=\"catering catering1\">";
                const string endTag = "<div class=\"catering catering1\">";
                var beginIndex = pageContent.IndexOf(beginTag, StringComparison.Ordinal);
                var endIndex = pageContent.IndexOf(endTag, beginIndex + 1, StringComparison.Ordinal);

                if (beginIndex >= endIndex)
                {
                    return null;
                }

                return pageContent.Substring(beginIndex, endIndex - beginIndex);
            }
        }
    }
}
