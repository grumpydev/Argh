namespace HttpClient
{
    using System;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Basic asynchronous http client
    /// </summary>
    public class AsyncHttpClient
    {
        /// <summary>
        /// Execute a web request asynchronously
        /// </summary>
        /// <param name="configuration">Web request configuration</param>
        /// <param name="onComplete">On complete callback</param>
        /// <param name="onError">On error callback</param>
        public void Execute(RequestConfiguration configuration, Action<HttpWebResponse> onComplete, Action<Exception> onError)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (onComplete == null)
            {
                throw new ArgumentNullException("onComplete");
            }

            if (onError == null)
            {
                throw new ArgumentNullException("onError");
            }

            if (String.IsNullOrWhiteSpace(configuration.Url))
            {
                throw new ArgumentException("Configuration must specify a URL", "configuration");
            }

            var request = this.CreateRequest(configuration);

            if (String.IsNullOrEmpty(configuration.RequestBody) || !this.CanSendBody(configuration))
            {
                // No body, just execute the request
                this.ExecuteRequestAsync(request, onComplete, onError);
                return;
            }

            // Body needs to be build async, otherwise the request will switch
            // to a blocking one for some stupid reason. 
            // MSDN doesn't seem to expliclty say you need to wait for the body to complete
            // before executing the request, but that's what their sample code does,
            // so better safe than sorry :-)
            this.BuildRequestBody(request, configuration.RequestBody, r => this.ExecuteRequestAsync(r, onComplete, onError), onError);
        }

        private bool CanSendBody(RequestConfiguration configuration)
        {
            return configuration.Method != HttpMethod.Get && configuration.Method != HttpMethod.Head;
        }

        private HttpWebRequest CreateRequest(RequestConfiguration configuration)
        {
            var request = (HttpWebRequest)WebRequest.Create(configuration.Url);
            request.UserAgent = configuration.UserAgent;
            request.Method = configuration.Method.ToString().ToUpper();

            foreach (var header in configuration.Headers)
            {
                request.Headers.Set(header.Key, header.Value);
            }

            request.ContentType = configuration.ContentType;

            if (configuration.NoCache)
            {
                request.Headers.Set("Pragma", "no-cache");
            }

            return request;
        }

        private void BuildRequestBody(HttpWebRequest request, string requestBody, Action<HttpWebRequest> onComplete, Action<Exception> onError)
        {
            request.BeginGetRequestStream(
                ar =>
                {
                    var state = (State<HttpWebRequest, HttpWebRequest>)ar.AsyncState;

                    try
                    {
                        using (var requestBodyStream = state.Source.EndGetRequestStream(ar))
                        {
                            var bodyBytes = Encoding.UTF8.GetBytes(requestBody);

                            requestBodyStream.Write(bodyBytes, 0, bodyBytes.Length);
                        }

                        state.OnComplete(state.Source);
                    }
                    catch (Exception e)
                    {
                        state.OnError.Invoke(e);
                        return;
                    }
                },
                new State<HttpWebRequest, HttpWebRequest>(request, onComplete, onError));
        }

        private void ExecuteRequestAsync(HttpWebRequest request, Action<HttpWebResponse> onComplete, Action<Exception> onError)
        {
            request.BeginGetResponse(
                ar =>
                {
                    var state = (State<HttpWebRequest, HttpWebResponse>)ar.AsyncState;

                    try
                    {
                        var response = (HttpWebResponse)state.Source.EndGetResponse(ar);
                        state.OnComplete.Invoke(response);
                    }
                    catch (Exception e)
                    {
                        state.OnError.Invoke(e);
                        return;
                    }
                },
                new State<HttpWebRequest, HttpWebResponse>(request, onComplete, onError));
        }

        /// <summary>
        /// Async callback state class
        /// </summary>
        /// <typeparam name="TSource">Source object type (the type with the Begin* method)</typeparam>
        /// <typeparam name="TPayload">The type to pass to the OnComplete delegate</typeparam>
        private class State<TSource, TPayload>
        {
            public readonly Action<TPayload> OnComplete;

            public readonly Action<Exception> OnError;

            public readonly TSource Source;

            /// <summary>
            /// Initializes a new instance of the State{TSource, TPayload} class. 
            /// </summary>
            /// <param name="source">Source object (the object that the Begin* method was called on)</param>
            /// <param name="onComplete">On complete callback</param>
            /// <param name="onError">On error callback</param>
            public State(TSource source, Action<TPayload> onComplete, Action<Exception> onError)
            {
                this.OnComplete = onComplete;
                this.OnError = onError;
                this.Source = source;
            }
        }
    }
}