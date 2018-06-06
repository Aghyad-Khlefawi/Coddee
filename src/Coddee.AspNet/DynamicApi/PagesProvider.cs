using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Coddee.AspNet
{
    /// <summary>
    /// Error and information pages names.
    /// </summary>
    public static class CoddeePages
    {
        /// <summary>
        /// An error page template.
        /// </summary>
        public const string Error = "Pages/Error.html";

        /// <summary>
        /// And error page with exception details.
        /// </summary>
        public const string ErrorDetailed = "Pages/ErrorWithDetailes.html";
    }

    /// <summary>
    /// Returns helper HTML pages for <see cref="DynamicApi"/>
    /// </summary>
    public class PagesProvider
    {
        /// <inheritdoc />
        public PagesProvider()
        {
            _cache = new Dictionary<string, string>();
        }

        private readonly Dictionary<string, string> _cache;

        /// <summary>
        /// Retrieve the content of an HTML page from the assembly
        /// </summary>
        /// <param name="name">the resource name</param>
        /// <returns></returns>
        public string GetPage(string name)
        {
            if (_cache.ContainsKey(name))
                return _cache[name];


            var assembly = Assembly.GetCallingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(FormatResourceName(assembly, name)))
            {
                if (stream == null)
                    return null;
                using (StreamReader reader = new StreamReader(stream))
                {
                    var res = reader.ReadToEnd();
                    _cache[name] = res;
                    return res;
                }
            }
        }

        private static string FormatResourceName(Assembly assembly, string resourceName)
        {
            return assembly.GetName().Name + "." + resourceName.Replace(" ", "_")
                                                               .Replace("\\", ".")
                                                               .Replace("/", ".");
        }

        /// <summary>
        /// Return HTML for an error page
        /// </summary>
        /// <returns></returns>
        public string GetErrorPage(int statusCode, string content)
        {
            string page = content != null ? GetPage(CoddeePages.ErrorDetailed) : GetPage(CoddeePages.Error); ;
            if (content != null)
            {
                content = content.Trim('\n').Replace(">", "&gt").Replace("<", "&lt");
                page = page.Replace("$Details$", content);
            }

            page = page.Replace("$StatusCode$", statusCode.ToString())
                       .Replace("$StatusTitle$", GetStatusTitle(statusCode));

            return page;
        }

        private string GetStatusTitle(int statusCode)
        {
            switch (statusCode)
            {
                case StatusCodes.Status401Unauthorized:
                    return "Unauthorized.";
                case DynamicApiExceptionCodes.ActionNotFound:
                    return "Not found.";
                case StatusCodes.Status400BadRequest:
                    return "Bad request.";
                default:
                    return "Internal server error.";
            }
        }

    }
}
