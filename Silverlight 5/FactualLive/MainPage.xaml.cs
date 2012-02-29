using System.Windows;
using System.Windows.Controls;
using FactualLive.Models;
using FactualLive.ViewModels;
using UpdateControls.XAML;

namespace FactualLive
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = ForView.Wrap(new MainViewModel(new FactualSession()));
        }
    }
}
