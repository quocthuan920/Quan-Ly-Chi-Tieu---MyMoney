using System.Threading.Tasks;
using System.Windows.Input;

namespace MyMoney.Ui.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();

        bool CanExecute();
    }

    public interface IAsyncCommand<in T> : ICommand
    {
        Task ExecuteAsync(T parameter);

        bool CanExecute(T parameter);
    }
}
