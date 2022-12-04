namespace shopping_bag.Config
{
    public static class StaticConfig
    {
        #region Jwt
        public static int RefreshTokenValidityDays { get; private set; }
        public static int TokenValidityInMinutes { get; private set; }
        public static string Token { get; private set; }
        public static string Issuer { get; private set; }
        public static string Audience { get; private set; }
        #endregion
        #region CORS
        public static string[] AllowedOrigins { get; private set; }
        public static string[] AllowedEmailDomain { get; private set; }
        #endregion
        #region Verification Email
        public static string VerificationEmailBodyText { get; private set; }
        #endregion

        #region Recover Email
        public static string RecoveryEmailBodyText  { get; private set; }
        #endregion
        #region Default Admin
        public static string DefaultAdminEmail { get; private set; }
        public static string DefaultAdminPassword { get; private set; }
        public static long DefaultAdminOfficeId { get; private set; }
        public static string[] DefaultAdminRoles { get; private set; }
        #endregion
        #region Reminders
        public static string EmailDueDateReminderFormat { get; private set; }
        public static string EmailExpectedDateReminderFormat { get; private set; }
        public static string EmailReminderIntro { get; private set; }
        public static string EmailReminderTurnOffEmails { get; private set; }
        public static int OrderedRemindersCleanUpDays { get; private set; }
        #endregion
        public static void Setup(IConfiguration config)
        {
            #region Jwt
            RefreshTokenValidityDays = config.GetValue<int>("Jwt:RefreshTokenValidityDays");
            TokenValidityInMinutes = config.GetValue<int>("Jwt:TokenValidityMinutes");
            Token = config.GetValue<string>("Jwt:Token");
            Issuer = config.GetValue<string>("Jwt:Issuer");
            Audience = config.GetValue<string>("Jwt:Audience");
            #endregion
            #region CORS
            AllowedOrigins = config.GetSection("AllowedOrigins").Get<string[]>();
            AllowedEmailDomain = config.GetSection("AllowedEmailDomain").Get<string[]>();
            #endregion
            #region Verification Email
            VerificationEmailBodyText = config.GetValue<string>("VerificationEmail:BodyText");
            #endregion
            #region Recover Email
            RecoveryEmailBodyText = config.GetValue<string>("RecoverEmail:BodyText");
            #endregion
            #region Default Admin
            DefaultAdminEmail = config.GetValue<string>("DefaultAdmin:Email");
            DefaultAdminPassword = config.GetValue<string>("DefaultAdmin:Password");
            DefaultAdminOfficeId = config.GetValue<long>("DefaultAdmin:OfficeId");
            DefaultAdminRoles = config.GetSection("DefaultAdmin:Roles").Get<string[]>();
            #endregion
            #region Reminders
            EmailDueDateReminderFormat = config.GetValue<string>("Reminders:DueDateReminderFormat");
            EmailExpectedDateReminderFormat = config.GetValue<string>("Reminders:ExpectedDateReminderFormat");
            EmailReminderIntro = config.GetValue<string>("Reminders:EmailReminderIntro");
            EmailReminderTurnOffEmails = config.GetValue<string>("Reminders:EmailReminderTurnOffEmails");
            OrderedRemindersCleanUpDays = config.GetValue<int>("Reminders:OrderedRemindersCleanUpDays");
            #endregion
        }
    }
}
