using System.Printing;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace RegViewer
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _keyHoldTimer;
        private Key? _currentHeldKey;
        private const int _keyHoldDelay = 350;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Item.BindingParam;
        }

        /// <summary>
        /// ウィンドウ全体のキー入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat && _currentHeldKey.HasValue) return;

            switch (e.Key)
            {
                case Key.Escape:
                    //  アプリケーションを終了するためのキー入力処理
                    _currentHeldKey = e.Key;
                    _keyHoldTimer = new DispatcherTimer();
                    _keyHoldTimer.Interval = TimeSpan.FromMilliseconds(_keyHoldDelay);
                    _keyHoldTimer.Tick += (sender, e) =>
                    {
                        Application.Current.Shutdown();
                        _keyHoldTimer?.Stop();
                    };
                    _keyHoldTimer.Start();
                    break;
            }
        }

        /// <summary>
        /// キーが離されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == _currentHeldKey)
            {
                _keyHoldTimer?.Stop();
                _currentHeldKey = null;
            }
        }

        /// <summary>
        /// GridSplitterのドラッグが完了したときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var grid = (sender as FrameworkElement)?.Parent as System.Windows.Controls.Grid;
            if (grid != null && grid.ColumnDefinitions.Count > 0)
            {
                var width = grid.ColumnDefinitions[0].ActualWidth;
                Item.BindingParam.Setting.TreeWidth = (int)width;
                //Item.BindingParam.Setting.Save();
            }
        }
    }
}
