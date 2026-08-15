namespace UnityFigmaMCP.Server.Figma
{
    public sealed class FigmaStatus
    {
        public bool TokenConfigured { get; set; }
        public bool? TokenValid { get; set; }

        public string? UserHandle { get; set; }
        public string? UserEmail { get; set; }
        public string? Error { get; set; }
    }
}
