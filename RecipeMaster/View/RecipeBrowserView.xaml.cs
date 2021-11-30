using RecipeMaster.Interface;
using RecipeMaster.Model;
using RecipeMaster.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RecipeMaster.View
{
    /// <summary>
    /// Interaction logic for RecipeBrowserView.xaml
    /// </summary>
    public partial class RecipeBrowserView : UserControl, ICloseableView
    {
        public RecipeBrowserView()
        {
            InitializeComponent();
        }
        public RecipeBrowserView(List<Recipe> recipes)
        {
            InitializeComponent();
            DataContext = RecipeBrowserViewModel.Create(recipes);
        }
        public void SetCloseAction(Action closeAction)
        {
            (DataContext as RecipeBrowserViewModel).SetCloseAction(closeAction);
        }
    }
}
