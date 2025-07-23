using System;

namespace API.Utility
{
    public static class SD
    {
        // Cookie
        public static readonly string IdentityAppCookie = "identityappcookie";

        // Application Claims
        public const string UserId = "uid";
        public const string Name = "name";
        public const string UserName = "username";
        public const string Email = "email";


        // Regext
        public const string UserNameRegex = "^[a-zA-Z][a-zA-Z0-9]*$";
        public const string EmailRegex = "^.+@[^\\.].*\\.[a-z]{2,}$";

        // Application rules
        public const int RequiredPasswordLength = 6;
        public const int MaxFailedAccessAttempts = 3;
        public const int DefaultLockoutTimeSpanInDays = 1;

        // Default Password
        public const string DefaultPassword = "123456";


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
    }
}
