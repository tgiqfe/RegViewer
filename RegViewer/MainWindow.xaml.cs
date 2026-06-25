using System.Windows;

namespace RegViewer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Item.BindingParam;
        }
    }
}