using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace MyMoney.Application.Common.CloudBackup
{
    public interface IGraphClientFactory
    {
        GraphServiceClient CreateClient(AuthenticationResult authResult);
    }
}
