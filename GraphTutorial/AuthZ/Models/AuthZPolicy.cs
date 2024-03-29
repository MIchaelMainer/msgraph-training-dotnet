﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace GraphTutorial.AuthZ.Models
{
    public enum AuthZPolicy
    {
        Base,
        AllowUsersPath,
        DisallowAllButOneWritePath,
        DisallowAllButTwoWritePaths,
        DisallowGETsWithoutSelect
    }
}
