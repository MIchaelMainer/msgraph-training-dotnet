// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Claims;

namespace GraphTutorial.AuthZ.Models
{
    internal class MsJwtClaims
    {
        public string? aud { get; set; }
        public string? iss { get; set; }
        public string? iat { get; set; }
        public string? nbf { get; set; }
        public string? exp { get; set; }
        public string? aad { get; set; }
        public string? oid { get; set; }
        public string? tid { get; set; }
        public string? ver { get; set; }
        public string? uti { get; set; }
        public string? sub { get; set; }
        public string? scp { get; set; }
        public string? appid { get; set; }
        public string? appidacr { get; set; }
        public string? idp { get; set; }
        public string? amr { get; set; }
        public string? family_name { get; set; }
        public string? given_name { get; set; }
        public string? ipaddr { get; set; }
        public string? name { get; set; }
        public string? puid { get; set; }
        public string? unique_name { get; set; }
        public string? upn { get; set; }
        public IList<string>? wids { get; set; }

        public MsJwtClaims(IEnumerable<Claim> claims)
        {
            foreach (Claim claim in claims)
            {
                // TODO: Populate other claims.
                switch (claim.Type)
                {
                    case "oid":
                        this.oid = claim.Value;
                        break;
                    case "tid":
                        this.tid = claim.Value;
                        break;
                    case "name":
                        this.name = claim.Value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
