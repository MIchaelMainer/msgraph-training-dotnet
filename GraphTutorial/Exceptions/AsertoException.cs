// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Graph;

namespace GraphTutorial.Exceptions
{
    public class AsertoException : Exception
    {
        public AsertoException(Error error, Exception? innerException = null)
            : this(error,
                   responseHeaders: null,
                   statusCode: default,
                   innerException: innerException)
        {
        }

        public AsertoException(Error error, System.Net.Http.Headers.HttpResponseHeaders responseHeaders, System.Net.HttpStatusCode statusCode, Exception? innerException = null)
            : base(error?.ToString(), innerException)
        {
            this.Error = error;
            this.ResponseHeaders = responseHeaders;
            this.StatusCode = statusCode;
        }

        public Error Error { get; }

        public System.Net.Http.Headers.HttpResponseHeaders ResponseHeaders { get; }

        public System.Net.HttpStatusCode StatusCode { get; }

    }
}
