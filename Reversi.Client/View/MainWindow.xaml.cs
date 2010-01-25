using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Reversi.Client.ViewModel;
using UpdateControls.XAML;

namespace Reversi.Client.View
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Square_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement control = sender as FrameworkElement;
            if (control != null)
            {
                SquareViewModel viewModel = ForView.Unwrap<SquareViewModel>(control.DataContext);
                if (viewModel != null)
                    viewModel.OnClick();
            }
        }
    }
}
