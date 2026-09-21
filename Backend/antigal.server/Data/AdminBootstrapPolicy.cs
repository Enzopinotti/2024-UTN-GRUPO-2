namespace antigal.server.Data
{
    public sealed record AdminBootstrapCredentials(
        string UserName,
        string Email,
        string Password);

    public static class AdminBootstrapPolicy
    {
        public static AdminBootstrapCredentials? Resolve(
            AdminBootstrapOptions options,
            bool isDevelopment)
        {
            ArgumentNullException.ThrowIfNull(options);

            if (!options.Enabled)
            {
                return null;
            }

            if (!isDevelopment)
            {
                throw new InvalidOperationException(
                    "El bootstrap de administrador sólo puede habilitarse en Development.");
            }

            if (string.IsNullOrWhiteSpace(options.UserName) ||
                string.IsNullOrWhiteSpace(options.Email) ||
                string.IsNullOrWhiteSpace(options.Password))
            {
                throw new InvalidOperationException(
                    "BootstrapAdmin requiere UserName, Email y Password cuando está habilitado.");
            }

            return new AdminBootstrapCredentials(
                options.UserName,
                options.Email,
                options.Password);
        }
    }
}
