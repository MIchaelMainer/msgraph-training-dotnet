using System.Text.Json.Serialization;

namespace GraphTutorial.AuthZ.Models
{
    internal class ContextPayload
    {

        [JsonPropertyName("identity_context")]
        public IdentityContext IdentityContext { get; set; }

        [JsonPropertyName("policy_context")]
        public PolicyContext PolicyContext { get; set; }
    }

    /// <summary>
    /// Identifies the user and user type. Used by the Authorizer to identify the user.
    /// https://docs.aserto.com/docs/authorizer-guide/identity-context
    /// </summary>
    internal class IdentityContext
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

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
}
