namespace HttpClient
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the configuration for a http request.
    /// </summary>
    public class RequestConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// Defaults to GET, AsyncHttpClient UA, No caching and a standard accept header.
        /// </summary>
        public RequestConfiguration()
        {
            this.Method = HttpMethod.Get;
            this.UserAgent = "AsyncHttpClient/1.0 (Windows; U; Windows NT 6.1; en-GB)";
            this.NoCache = true;
            this.Headers = new Dictionary<string, string>();
            this.AcceptHeader = "application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
        }

        /// <summary>
        /// Gets or sets the http method.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Gets or sets user agent string.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to send a pragma nocache or not.
        /// </summary>
        public bool NoCache { get; set; }

        /// <summary>
        /// Gets or sets the request URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the accept header string
        /// </summary>
        public string AcceptHeader { get; set; }

        /// <summary>
        /// Gets or sets the request body
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// Gets or sets the request content type
        /// </summary>
        public string ContentType { get; set; }
    }
}