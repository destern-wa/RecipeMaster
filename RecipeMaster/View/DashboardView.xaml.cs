using RecipeMaster.ViewModel;
using System.Windows.Controls;

namespace RecipeMaster.View
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = DashboardViewModel.Create();
        }
    }
}
