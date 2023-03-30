using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServicePrincipalsSamples
{
    public class AdoRequestHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Request: {request.Method} {request.RequestUri}\n");
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
