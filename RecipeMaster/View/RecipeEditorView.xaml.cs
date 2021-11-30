using RecipeMaster.Interface;
using RecipeMaster.Model;
using RecipeMaster.ViewModel;
using System;
using System.Windows.Controls;

namespace RecipeMaster.View
{
    /// <summary>
    /// Interaction logic for RecipeEditorView.xaml
    /// </summary>
    public partial class RecipeEditorView : UserControl, ICloseableView
    {
        public RecipeEditorView()
        {
            InitializeComponent();
            DataContext = RecipeEditorViewModel.Create();
        }

        public RecipeEditorView(Recipe recipe)
        {
            InitializeComponent();
            DataContext = RecipeEditorViewModel.Create(recipe);
        }

        public void SetCloseAction(Action closeAction)
        {
            (DataContext as RecipeEditorViewModel).SetCloseAction(closeAction);
        }
    }
}
