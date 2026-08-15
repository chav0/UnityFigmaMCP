using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFigmaMCP.Server.Figma
{
    internal sealed class FigmaAuthService : IFigmaAuthService
    {
        private readonly IFigmaApiClient _client;

        public FigmaAuthService(IFigmaApiClient client) => _client = client;

        public async Task<FigmaStatus> GetStatusAsync(CancellationToken ct)
        {
            if (!FigmaToken.IsSet)
                return new FigmaStatus { TokenConfigured = false };

            try
            {
                var user = await _client.GetCurrentUserAsync(ct).ConfigureAwait(false);

                return new FigmaStatus
                {
                    TokenConfigured = true,
                    TokenValid = true,
                    UserHandle = user?.handle,
                    UserEmail = user?.email
                };
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return new FigmaStatus
                {
                    TokenConfigured = true,
                    TokenValid = false,
                    Error = ex.ToDescription()
                };
            }
        }
    }
}
