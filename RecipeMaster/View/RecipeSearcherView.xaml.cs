using RecipeMaster.Interface;
using RecipeMaster.ViewModel;
using System;
using System.Windows.Controls;

namespace RecipeMaster.View
{
    /// <summary>
    /// Interaction logic for RecipeSearcherView.xaml
    /// </summary>
    public partial class RecipeSearcherView : UserControl, ICloseableView
    {
        public RecipeSearcherView()
        {
            InitializeComponent();
            DataContext = RecipeSearcherViewModel.Create();
            ResultsBrowserView.DataContext = RecipeBrowserViewModel.Create(null);
        }

        public void SetCloseAction(Action closeAction)
        {
            (DataContext as RecipeSearcherViewModel).SetCloseAction(closeAction);
        }
    }
}
