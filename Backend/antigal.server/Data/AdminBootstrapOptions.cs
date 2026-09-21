namespace antigal.server.Data
{
    public sealed class AdminBootstrapOptions
    {
        public const string SectionName = "BootstrapAdmin";

        public bool Enabled { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
