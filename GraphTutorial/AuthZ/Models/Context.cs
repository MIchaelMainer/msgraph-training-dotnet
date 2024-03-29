﻿using GraphTutorial.Http.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphTutorial.AuthZ.Models
{
    /// <summary>
    /// Models the payload sent to the Authorizer Is API.
    /// https://docs.aserto.com/docs/authorizer-guide/is
    /// </summary>
    internal class ContextPayload
    {
        [JsonPropertyName("identity_context")]
        public IdentityContext IdentityContext { get; set; }

        [JsonPropertyName("policy_context")]
        public PolicyContext PolicyContext { get; set; }

        [JsonPropertyName("resource_context")]
        public IDictionary<string, object>? ResourceContext { get; set; }
    }

    /// <summary>
    /// Identifies the user and user type. Used by the Authorizer to identify the user.
    /// https://docs.aserto.com/docs/authorizer-guide/identity-context
    /// </summary>
    internal class IdentityContext
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("type")]
        public IdentityType Type { get; set; }

        [JsonPropertyName("identity")]
        public string Identity { get; set; }
    }

    /// <summary>
    /// Identifies the policy and the decisions to evaluate.
    /// https://docs.aserto.com/docs/authorizer-guide/policy-context
    /// </summary>
    internal class PolicyContext
    {

        [JsonPropertyName("decisions")]
        public List<string> Decisions { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
    }

    /// <summary>
    /// Identifies the ResourceContext
    /// https://docs.aserto.com/docs/authorizer-guide/resource-context
    /// </summary>
    /// 
    //internal class ResourceContext
    //{
    //    [JsonPropertyName("httprequest")]
    //    public HttpRequestMessageModel? HttpRequest { get; set; }
    //}

    /// <summary>
    /// Models the results returned from the Authorizer.
    /// </summary>
    internal class DecisionResult
    {
        [JsonPropertyName("decisions")]
        public List<Decision> Decisions { get; set; }
    }

    /// <summary>
    /// Represents a single decision returned by the Authorizer Is API.
    /// </summary>
    internal class Decision
    {
        [JsonPropertyName("decision")]
        public string DecisionType { get; set; } // ugh <-- forgot why I wrote this, something about naming probably

        [JsonPropertyName("is")]
        public bool Is { get; set; }
    }

    internal enum IdentityType
    {
        IDENTITY_TYPE_NONE,
        IDENTITY_TYPE_SUB,
        IDENTITY_TYPE_JWT
    }

    internal class PoliciesResult
    {
        [JsonPropertyName("results")]
        public List<Policy> Results { get; set; }
    }

    internal class Policy
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
