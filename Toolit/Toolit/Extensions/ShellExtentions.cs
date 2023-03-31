using Toolit.Interfaces;
using Xamarin.Forms;

namespace Toolit.Extensions
{
    public static class ShellExtensions
    {
        public static Page GetCurrentPage(this Shell shell)
        {
            return (shell?.CurrentItem?.CurrentItem as IShellSectionController)?.PresentedPage;
        }

        public static INavigationHandler GetCurrentViewModel(this Shell shell)
        {
            var page = shell.GetCurrentPage();

            if (page != null && page.BindingContext
                is INavigationHandler viewModel)
            {
                return viewModel;
            }

            return null;
        }
    }
}