// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IdentityModel.Tokens.Jwt;

namespace GraphTutorial.AuthZ.Utils
{
    internal static class JwtDecoder
    {
        public static JwtPayload? DecodeJWT(string jwt)
        {
            JwtSecurityTokenHandler handler = new();
            return handler.CanValidateToken && handler.CanReadToken(jwt) ? handler.ReadJwtToken(jwt).Payload : null;
        }
    }
}
