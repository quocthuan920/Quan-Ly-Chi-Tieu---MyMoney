using System.Threading.Tasks;

namespace MyMoney.Services
{
    public interface IToastService
    {
        Task ShowToastAsync(string message, string title = "");
    }
}
