using Microsoft.Graph;
using Microsoft.Identity.Client;
using MyMoney.Application.Common.CloudBackup;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyMoney.Droid.Src
{
    public class GraphClientFactory : IGraphClientFactory
    {
        public GraphServiceClient CreateClient(AuthenticationResult authResult)
        {
            return new GraphServiceClient(new DelegateAuthenticationProvider(requestMessage =>
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", authResult.AccessToken);
                return Task.CompletedTask;
            }));
        }
    }
}
