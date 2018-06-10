namespace Coddee
{
    /// <summary>
    /// HTTP methods.
    /// </summary>
    public static class HttpMethod
    {
        /// <summary>
        /// GET Http method.
        /// </summary>
        public const string Get = "GET";
        /// <summary>
        /// POST Http method.
        /// </summary>
        public const string Post = "POST";
        /// <summary>
        /// PUT Http method.
        /// </summary>
        public const string Put = "PUT";
        /// <summary>
        /// DELETE Http method.
        /// </summary>
        public const string Delete = "DELETE";

        /// <summary>
        /// Compares the method to GET method.
        /// </summary>
        public static bool IsGet(string method)
        {
            return method.ToUpper() == Get;
        }

        /// <summary>
        /// Compares the method to POST method.
        /// </summary>

        public static bool IsPost(string method)
        {
            return method.ToUpper() == Post;
        }

        /// <summary>
        /// Compares the method to PUT method.
        /// </summary>
        public static bool IsPut(string method)
        {
            return method.ToUpper() == Put;
        }

        /// <summary>
        /// Compares the method to DELETE method.
        /// </summary>
        public static bool IsDelete(string method)
        {
            return method.ToUpper() == Delete;
        }
    }
}