// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using GraphTutorial.AuthZ.Models;
using GraphTutorial.AuthZ.Utils;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GraphTutorial.AuthZ
{
    public class AzuthZHandler : DelegatingHandler
    {
        static readonly string directoryToken = Environment.GetEnvironmentVariable("asertoDirectoryToken");
        static readonly string authorizerToken = Environment.GetEnvironmentVariable("asertoAuthorizerToken");
        static readonly string asertoTenant = Environment.GetEnvironmentVariable("asertoTenant");
        static readonly HttpClient client = new();

        public AzuthZHandler() { }

        public AzuthZHandler(HttpMessageHandler innerHandler)
        {
            this.InnerHandler = innerHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // TODO:
            // 1. Get the user's ID from the JWT.
            // 2. Call authorizer to check if the user is allowed to make the call.
            if (string.IsNullOrWhiteSpace(request.Headers?.Authorization?.Parameter))
            {
                throw new Exception("Authorization header is missing.");
            }
            var JwtPayload = JwtDecoder.DecodeJWT(request.Headers.Authorization.Parameter);
            var msJwtClaims = new MsJwtClaims(JwtPayload?.Claims);

            return !(await IsAuthorizedAsync(msJwtClaims.oid))
                ? throw new Exception("Access denied!")
                : await base.SendAsync(request, cancellationToken);
        }

        static async Task<bool> IsAuthorizedAsync(string userId)
        {
            // TODO: Validate call.
            if (string.IsNullOrWhiteSpace(userId))
                return false;
            var request = new HttpRequestMessage(HttpMethod.Post, "https://authorizer.prod.aserto.com/api/v1/authz/is");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", directoryToken);
            request.Headers.Add("aserto-tenant-id", asertoTenant);

            ContextPayload contextPayload = new()
            {
                IdentityContext = new IdentityContext()
                {
                    Type = "IDENTITY_TYPE_SUB",
                    Identity = userId
                },
                PolicyContext = new PolicyContext()
                {
                    Decisions = new List<string>() { "allowed" },
                    Id = "18cdef8a-acc3-11ed-9581-01777bcce0c6",
                    Path = "simplepolicy.GET.me"
                }
            };

            var payload = JsonSerializer.Serialize(contextPayload);
            request.Content = new StringContent(payload, Encoding.UTF8, request.Headers.Accept.ToString());
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return false;

            var result = JsonSerializer.Deserialize<DecisionResult>(await response.Content.ReadAsStreamAsync());
            // Opportunity to improve by getting the decisions in a single response.
            return result.Decisions[0].Is;
        }
    }
}