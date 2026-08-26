using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Infrastructure.Services.Email.Templates
{
    public static class PasswordResetEmailTemplate
    {

        public const string ApplicationName = "Pharmacy Inventory System";
       public static string Build(string resetUrl)
        {
            return $"""
            <h2>Password Reset</h2>

            <p>
                We received a request to reset your password.
            </p>

            <p>
                Click the link below to reset your password:
            </p>

            <p>
                <a href="{resetUrl}">
                    Reset Password
                </a>
            </p>

            <p>
                If you did not request a password reset, you can safely ignore this email.
            </p>

            <p>
            Thank you,<br/>
            {ApplicationName}

            </p>

            """;


        }    
    }
}
