namespace Argh.DSL
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using HttpClient;

    public abstract class ArghSettings
    {
        private List<RequestConfiguration> requestConfigurations = new List<RequestConfiguration>();

        private RequestConfiguration currentRequestConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        protected ArghSettings()
        {
            this.Iterations = 5000;
        }

        public IEnumerable<RequestConfiguration> RequestConfigurations
        {
            get
            {
                return this.requestConfigurations.AsReadOnly();
            }
        }

        /// <summary>
        /// Number of iterations to run
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// The build method is the method that the settings DSL code is inserted into.
        /// </summary>
        public abstract void Build();

        protected void RequestConfiguration(string name, Action action)
        {
            this.currentRequestConfiguration = new RequestConfiguration() { Name = name };
            this.requestConfigurations.Add(this.currentRequestConfiguration);
            action();
        }

        public void Method(string method)
        {
            if (this.currentRequestConfiguration == null)
            {
                return;
            }

            this.currentRequestConfiguration.Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), method, true);
        }

        public void UserAgent(string userAgent)
        {
            if (this.currentRequestConfiguration == null)
            {
                return;
            }

            this.currentRequestConfiguration.UserAgent = userAgent;
        }

        public void NoCache(bool noCache)
        {
            if (this.currentRequestConfiguration == null)
            {
                return;
            }

            this.currentRequestConfiguration.NoCache = noCache;
        }

        public void Url(string url)
        {
            if (this.currentRequestConfiguration == null)
            {
                return;
            }

            this.currentRequestConfiguration.Url = url;
        }

        public void ContentType(string contentType)
        {
            if (this.currentRequestConfiguration == null)
            {
                return;
            }

            this.currentRequestConfiguration.ContentType = contentType;
        }

        public void Headers(IDictionary headers)
        {
            if (this.currentRequestConfiguration == null)
            {
                return;
            }

            foreach (var header in headers.Keys)
            {
                this.currentRequestConfiguration.Headers.Add(header.ToString(), headers[header].ToString());
            }
        }

        public void PostForm(IDictionary formElements)
        {
            if (this.currentRequestConfiguration == null)
            {
                return;
            }

            var sb = new StringBuilder();
            foreach (var formElement in formElements.Keys)
            {
                sb.AppendFormat("{0}={1}&", Helpers.HttpUtility.UrlEncode(formElement.ToString()), Helpers.HttpUtility.UrlEncode(formElements[formElement].ToString()));
            }

            this.currentRequestConfiguration.ContentType = @"application/x-www-form-urlencoded";
            this.currentRequestConfiguration.RequestBody = sb.ToString();
        }

        public void Body(string body)
        {
            if (this.currentRequestConfiguration == null)
            {
                return;
            }

            this.currentRequestConfiguration.RequestBody = body;
        }
    }
}