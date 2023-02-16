// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using GraphTutorial.AuthZ;
using GraphTutorial.AuthZ.Models;
using Microsoft.Graph;

namespace GraphTutorial
{
    public static class BaseRequestExtentions
    {
        public static T WithAuthZPolicy<T>(this T baseRequest, AuthZPolicy policy) where T : IBaseRequest
        {
            string authZOptionKey = typeof(AuthZHandlerOption).Name;
            if (baseRequest.MiddlewareOptions.ContainsKey(authZOptionKey))
            {
                ((AuthZHandlerOption)baseRequest.MiddlewareOptions[authZOptionKey]).Policy = policy;
            }
            else
            {
                baseRequest.MiddlewareOptions.Add(authZOptionKey, new AuthZHandlerOption { Policy = policy });
            }
            return baseRequest;
        }
    }
}
