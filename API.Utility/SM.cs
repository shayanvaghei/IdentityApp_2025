namespace API.Utility
{
    public static class SM
    {
        // Titles
        public const string T_AccountCreated = "Account created";
        public const string T_EmailSent = "Email sent";
        public const string T_AccountSuspended = "Account suspended";
        public const string T_InvallidToken = "Invalid token";
        public const string T_EmailSentFailed = "Failed to send email";
        public const string T_EmailConfirmed = "Email confirmed";
        public const string T_ConfirmEmailFirst = "Confirm your email first";
        public const string T_PasswordRest = "Password reset";
        public const string T_AccountWasConfirmed = "Email was confirmed";


        // Messages
        public const string M_AccountCreated = "Your account has been created, please confrim your email address.";
        public const string M_InavlidToken = "Invalid token. Please try again.";
        public const string M_AccountSuspended = "Your account has been suspended. Please contact our site administrator for more information.";
        public const string M_ForgotUsernamePasswordSent = "If an eligible account exists, we've sent password reset instructions to its primary email. Check your inbox or spam folder.";
        public const string M_ConfirmEmailSend = "If an eligible account exists, we've sent account activation instructions to its primary email. Check your inbox or spam folder.";
        public const string M_AccountWasConfirmed = "Your email has already been confirmed. Please log in to your account.";
        public const string M_EmailSentFailed = "Failed to send the email. Please contact the administrator.";
        public const string M_EmailConfirmed = "Your email address has been confirmed. You can log in now.";
        public const string M_ConfirmEmailFirst = "Your account isn’t confirmed yet. Please confirm your email address first.";
        public const string M_PasswordRest = "Your password has been reset.";
        public const string M_MfaDisableEmailSent = "If an eligible account exists, we've sent mfa disable instructions to its primary email. Check your inbox or spam folder.";
    }
}
