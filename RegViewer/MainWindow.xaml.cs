using System.Printing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace RegViewer
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _keyHoldTimer;
        private Key? _currentHeldKey;
        private const int _keyHoldDelay = 300;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Item.BindingParam;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    _currentHeldKey = e.Key;
                    _keyHoldTimer = new DispatcherTimer();
                    _keyHoldTimer.Interval = TimeSpan.FromMilliseconds(_keyHoldDelay);
                    _keyHoldTimer.Tick += (sender, e) =>
                    {
                        Application.Current.Shutdown(); 
                    };
                    _keyHoldTimer.Start();
                    break;
            }
        }
    }
}
