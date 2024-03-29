// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using GraphTutorial.AuthZ.Models;
using GraphTutorial.Exceptions;
using GraphTutorial.Http.Models;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GraphTutorial.AuthZ
{
    public class AuthZHandler : DelegatingHandler
    {
        static readonly string directoryToken = Environment.GetEnvironmentVariable("asertoDirectoryToken");
        static readonly string authorizerToken = Environment.GetEnvironmentVariable("asertoAuthorizerToken");
        static readonly string asertoTenant = Environment.GetEnvironmentVariable("asertoTenant");
        static readonly HttpClient client = new();

        public AuthZHandlerOption AuthZOption { get; set; }

        public AuthZHandler(AuthZHandlerOption? authZOption = null)
        {
            AuthZOption = authZOption ?? new AuthZHandlerOption();
        }

        public AuthZHandler(HttpMessageHandler innerHandler, AuthZHandlerOption? authZOption = null)
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
            var httpRequestMessageResource = new HttpRequestMessageModel(request);
            var policyName = AuthZOption.Policy.ToString();
            return !(await IsAuthorizedAsync(policyName, httpRequestMessageResource))
                ? throw new AuthorizationException("Access denied", policyName, httpRequestMessageResource)
                : await base.SendAsync(request, cancellationToken);
        }

        public static async Task<bool> IsAuthorizedAsync(string policyName, HttpRequestMessageModel httpRequestMessageModel)
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
                    Id = await GetLatestPolicyIdAsync().ConfigureAwait(false), // TODO: Cache this value
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

        static async Task<string> GetLatestPolicyIdAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://authorizer.prod.aserto.com/api/v1/policy/policies");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", directoryToken);
            request.Headers.Add("aserto-tenant-id", asertoTenant);

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new AsertoException(new Error
                {
                    Code = response.StatusCode.ToString(),
                    Message = await response.Content.ReadAsStringAsync()
                });

            var result = JsonSerializer.Deserialize<PoliciesResult>(await response.Content.ReadAsStreamAsync());
            return result.Results.Single(r => r.Name == "msgraph-simple-policy").Id;
        }
    }
}