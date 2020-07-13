using MVVM_SQlite_LocalDB.ViewModels;
using System.Windows;

namespace MVVM_SQlite_LocalDB.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
