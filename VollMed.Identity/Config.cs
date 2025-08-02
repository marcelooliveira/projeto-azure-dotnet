// Copyright (c) Duende Software. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityModel;

namespace VollMed.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource()
            {
                Name = "verification",
                UserClaims =
                [
                    JwtClaimTypes.Email,
                    JwtClaimTypes.EmailVerified
                ]
            }
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope(name: "VollMed.WebAPI", displayName: "VollMed.WebAPI")
        ];

    public static IEnumerable<Client> Clients =>
        [
            new Client
            {
                ClientId = "VollMed.Web",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                
                // where to redirect to after login
                RedirectUris = { "https://localhost:5002/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                AllowOfflineAccess = true,

                AllowedScopes =
                [
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "VollMed.WebAPI"
                ]
            }
        ];
}
