using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyInventoryDispensingSystem.Application.Common.Errors
{
    public static class UserErrors
    {
        public static Error NotFound(string userId) =>
            Error.NotFound(
                "Users.NotFound",
                $"User with ID '{userId}' was not found.");

        public static Error EmailConflict =>
            Error.Conflict(
                "Users.EmailConflict",
                "A user with this email already exists.");

        public static Error InvalidRole =>
            Error.Validation(
                "Users.InvalidRole",
                "The selected role is invalid.");

        public static Error AlreadyActive =>
            Error.Conflict(
                "Users.AlreadyActive",
                "The user account is already active.");

        public static Error AlreadyInactive =>
            Error.Conflict(
                "Users.AlreadyInactive",
                "The user account is already inactive.");
    }
}
