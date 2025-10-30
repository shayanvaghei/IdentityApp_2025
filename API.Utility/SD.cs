using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Utility
{
    public static class SD
    {
        private static readonly Random _random = new Random();

        // Cookie
        public static readonly string IdentityAppCookie = "identityappcookie";

        // Application roles
        public const string AdminRole = "admin";
        public const string UserRole = "user";
        public const string ModeratorRole = "moderator";
        public static readonly List<string> Roles = new List<string> { AdminRole, UserRole, ModeratorRole };

        //Super admin
        public const string SuperAdminUsername = "iaadmin";
        public const string SuperAdminEmail = "iaadmin@example.com";

        // Application Claims
        public const string UserId = "uid";
        public const string Name = "name";
        public const string UserName = "username";
        public const string Email = "eml";

        // Application Claims-Policy
        public const string AdminPolicy = "AdminPolicy";
        public const string ModeratorPolicy = "ModeratorPolicy";
        public const string UserPolicy = "UserPolicy";
        public const string AdminOrModeratorPolicy = "AdminOrModeratorPolicy";
        public const string AdminAndModeratorPolicy = "AdminAndModeratorPolicy";
        public const string AllRolePolicy = "AllRolePolicy";
        public const string AdminEmailPolicy = "AdminEmailPolicy";
        public const string VIPPolicy = "VIPPolicy";


        // Regext
        public const string UserNameRegex = "^[a-zA-Z][a-zA-Z0-9]*$";
        public const string EmailRegex = "^.+@[^\\.].*\\.[a-z]{2,}$";

        // Application rules
        public const int RequiredPasswordLength = 6;
        public const int MaxFailedAccessAttempts = 3;
        public const int DefaultLockoutTimeSpanInDays = 1;

        // Default Password
        public const string DefaultPassword = "123456";

        // Naming
        public const string EC = "ec"; // email confirmation
        public const string FUP = "fup"; // forgotusernamepassword
        public const string Authenticator = "authenticator";
        public const string MFAS = "mfas"; // multi-factor authentication secret
        public const string MFASDisable = "mfasdisable"; // multi-factor authentication secret

        public static string AccountLockedMessage(DateTime endDate)
        {
            DateTime startDate = DateTime.UtcNow;
            TimeSpan difference = endDate - startDate;

            int days = difference.Days;
            int hours = difference.Hours;
            int minutes = difference.Minutes + 1;

            return string.Format("Your account is temporary locked.<br>You should wait {0} day(s), {1} hour(s) and {2} minute(s)",
                days, hours, minutes);
        }

        public static string GenerateRandomString(int length = 6)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
