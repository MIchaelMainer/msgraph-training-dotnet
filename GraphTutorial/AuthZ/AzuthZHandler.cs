// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using GraphTutorial.AuthZ.Models;
using GraphTutorial.Exceptions;
using Microsoft.Graph;
using GraphTutorial.AuthZ.Utils;
using GraphTutorial.Http.Models;
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

        public AuthZHandlerOption AuthZOption { get; set; }

        public AzuthZHandler(AuthZHandlerOption? authZOption = null)
        {
            AuthZOption = authZOption ?? new AuthZHandlerOption();
        }

        public AzuthZHandler(HttpMessageHandler innerHandler, AuthZHandlerOption? authZOption = null)
            : this(authZOption)
        {
            this.InnerHandler = innerHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthZOption = request.GetMiddlewareOption<AuthZHandlerOption>() ?? AuthZOption;
            if (string.IsNullOrWhiteSpace(request.Headers?.Authorization?.Parameter))
                throw new Exception("Authorization header is missing.");
            //var JwtPayload = JwtDecoder.DecodeJWT(request.Headers.Authorization.Parameter);
            //var msJwtClaims = new MsJwtClaims(JwtPayload?.Claims);

            // Convert HttpRequestMessage into a model used for evaluation.
            var httpRequestMessageResource = new GraphTutorial.Http.Models.HttpRequestMessageModel(request);

            return !(await IsAuthorizedAsync(AuthZOption.Policy.ToString(), httpRequestMessageResource))
                ? throw new Exception("Access denied!")
                : await base.SendAsync(request, cancellationToken);
        }

        static async Task<bool> IsAuthorizedAsync(string policyName, HttpRequestMessageModel httpRequestMessageModel)
        {
            if (string.IsNullOrWhiteSpace(policyName))
                return false;
            var request = new HttpRequestMessage(HttpMethod.Post, "https://authorizer.prod.aserto.com/api/v1/authz/is");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", directoryToken);
            request.Headers.Add("aserto-tenant-id", asertoTenant);

            ContextPayload contextPayload = new()
            {
                IdentityContext = new IdentityContext()
                {
                    Type = IdentityType.IDENTITY_TYPE_NONE
                    //Identity = "011a88bc-7df9-4d92-ba1f-2ff319e101e1"
                },
                PolicyContext = new PolicyContext()
                {
                    Decisions = new List<string>() { "allow" },
                    Id = "3fc33fbf-ae53-11ed-904d-01777bcce0c6",
                    Path = policyName
                },
                ResourceContext = new Dictionary<string, object>()
                {
                    { "httpRequest", httpRequestMessageModel }
                }
                
            };

            var payload = JsonSerializer.Serialize(contextPayload);
            request.Content = new StringContent(payload, Encoding.UTF8, request.Headers.Accept.ToString());
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new AsertoException(new Error
                {
                    Code = response.StatusCode.ToString(),
                    Message = await response.Content.ReadAsStringAsync()
                });

            var result = JsonSerializer.Deserialize<DecisionResult>(await response.Content.ReadAsStreamAsync());
            // Opportunity to improve by getting the decisions in a single response.
            return result.Decisions[0].Is;
        }
    }
}