using System.Threading.Tasks;
using System.Windows.Input;

namespace AudioConverter.MVVM
{
    internal interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
