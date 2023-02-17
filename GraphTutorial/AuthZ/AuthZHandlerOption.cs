// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using GraphTutorial.AuthZ.Models;
using Microsoft.Graph;

namespace GraphTutorial.AuthZ
{
    public class AuthZHandlerOption : IMiddlewareOption
    {
        public AuthZHandlerOption() { }

        public AuthZPolicy Policy { get; set; }
    }
}
