// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using GraphTutorial.Http.Models;

namespace GraphTutorial.Exceptions
{
    internal class AuthorizationException : Exception
    {
        public AuthorizationException(string message, string policyName, HttpRequestMessageModel requestMessageModel, Exception? innerException = null)
            : base(message, innerException)
        {
            this.PolicyName = policyName;
            this.RequestMessageModel = requestMessageModel;
        }
        public string PolicyName { get; set; }
        public HttpRequestMessageModel RequestMessageModel { get; set; }

    }
}
