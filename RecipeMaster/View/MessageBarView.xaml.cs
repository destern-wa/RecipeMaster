using RecipeMaster.ViewModel;
using System.Windows.Controls;

namespace RecipeMaster.View
{
    /// <summary>
    /// Interaction logic for MessageBarView.xaml
    /// </summary>
    public partial class MessageBarView : UserControl
    {
        public MessageBarView()
        {
            InitializeComponent();
            if (DataContext==null)  DataContext = MessageBarViewModel.Create();
        }
    }
}
