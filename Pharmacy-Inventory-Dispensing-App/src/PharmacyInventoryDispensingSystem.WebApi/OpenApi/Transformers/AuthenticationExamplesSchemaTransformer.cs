using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using PharmacyInventoryDispensingSystem.Application.Features.SecurityManager.Authentication.DTOs;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.Requests.Authentication;
using System.Text.Json.Nodes;

namespace PharmacyInventoryDispensingSystem.WebApi.OpenApi.Transformers
{
    public sealed class AuthenticationExamplesSchemaTransformer
     : IOpenApiSchemaTransformer
    {
        public Task TransformAsync(
            OpenApiSchema schema,
            OpenApiSchemaTransformerContext context,
            CancellationToken cancellationToken)
        {
            Type schemaType = context.JsonTypeInfo.Type;

            if (schemaType == typeof(RegisterRequest))
            {
                schema.Example = Parse(
                    """
                {
                  "email": "pharmacist@pharmacy.local",
                  "password": "Pharmacist@123",
                  "firstName": "Ahmad",
                  "lastName": "Ali",
                  "role": "Pharmacist"
                }
                """);
            }
            else if (schemaType == typeof(LoginRequest))
            {
                schema.Example = Parse(
                    """
                {
                  "email": "admin@pharmacy.local",
                  "password": "Admin@12345!"
                }
                """);
            }
            else if (schemaType == typeof(RefreshTokenRequest))
            {
                schema.Example = Parse(
                    """
                {
                  "refreshToken": "refresh-token-returned-from-login"
                }
                """);
            }
            else if (schemaType == typeof(LogoutRequest))
            {
                schema.Example = Parse(
                    """
                {
                  "refreshToken": "refresh-token-returned-from-login"
                }
                """);
            }
            else if (schemaType == typeof(ChangePasswordRequest))
            {
                schema.Example = Parse(
                    """
                {
                  "currentPassword": "Admin@12345!",
                  "newPassword": "NewAdmin@456"
                }
                """);
            }
            else if (schemaType == typeof(ForgotPasswordRequest))
            {
                schema.Example = Parse(
                    """
                {
                  "email": "admin@pharmacy.local"
                }
                """);
            }
            else if (schemaType == typeof(ResetPasswordRequest))
            {
                schema.Example = Parse(
                    """
                {
                  "email": "admin@pharmacy.local",
                  "token": "password-reset-token",
                  "newPassword": "NewAdmin@456"
                }
                """);
            }
            else if (schemaType == typeof(AuthenticationResponse))
            {
                schema.Example = Parse(
                    """
                {
                  "userId": "42d9aabd-52e7-4a98-8865-c4e8d84427f1",
                  "email": "admin@pharmacy.local",
                  "firstName": "System",
                  "lastName": "Administrator",
                  "roles": [
                    "Admin"
                  ],
                  "permissions": [
                    "Permissions.Patients.Read",
                    "Permissions.Patients.Create",
                    "Permissions.Patients.Update",
                    "Permissions.Patients.Delete"
                  ],
                  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.example",
                  "accessTokenExpiresAtUtc": "2026-08-29T13:00:00Z",
                  "refreshToken": "generated-refresh-token-example",
                  "refreshTokenExpiresAtUtc": "2026-09-05T12:00:00Z"
                }
                """);
            }
            else if (schemaType == typeof(CurrentUserResponse))
            {
                schema.Example = Parse(
                    """
                {
                  "userId": "42d9aabd-52e7-4a98-8865-c4e8d84427f1",
                  "email": "admin@pharmacy.local",
                  "firstName": "System",
                  "lastName": "Administrator",
                  "roles": [
                    "Admin"
                  ],
                  "permissions": [
                    "Permissions.Patients.Read",
                    "Permissions.Patients.Create",
                    "Permissions.Patients.Update",
                    "Permissions.Patients.Delete"
                  ]
                }
                """);
            }

            return Task.CompletedTask;
        }

        private static JsonNode Parse(string json)
        {
            return JsonNode.Parse(json)!;
        }
    }
    }
