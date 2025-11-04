using System.Threading;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    public static class CancellationTokenExtensions
    {
        public static void ThrowIfCancelled(this CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                throw new TaskCanceledException("Client has disconnected.");
        }
    }
}
