using HiNote.Helpers;
using HiNote.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using mux = Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HiNote.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectTreeViewPage : Page
    {
        public SelectTreeViewModel ViewModel
        {
            get;
        }

        public SelectTreeViewPage()
        {
            ViewModel = App.GetService<SelectTreeViewModel>();
            this.InitializeComponent();
            ViewModel.LoadCategoryListAsync();
        }

        private async void categoryTreeView_ItemInvoked(mux.TreeView sender, mux.TreeViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem != null)
            {
                ViewModel.SelectedItem = args.InvokedItem as ExplorerItem;
            }
        }
    }
}
