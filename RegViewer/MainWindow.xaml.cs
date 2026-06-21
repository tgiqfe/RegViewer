using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RegViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = Item.BindingParam;

            Item.BindingParam.Keys1.Add(new KeyItem(Registry.CurrentUser));
            Item.BindingParam.Keys2.Add(new KeyItem(Registry.LocalMachine));
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is KeyItem selectedKeyItem)
            {
                AddressBar.Text = selectedKeyItem.Path;
                foreach (var item in selectedKeyItem.SubKeys)
                {
                    item.LoadSubKeys();
                }
            }
        }

        private void TreeView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (TreeScrollViewer != null)
            {
                TreeScrollViewer.ScrollToVerticalOffset(TreeScrollViewer.VerticalOffset - e.Delta / 3.0);
                e.Handled = true;
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem && treeViewItem.DataContext is KeyItem expandingKeyItem)
            {
                foreach (var item in expandingKeyItem.SubKeys)
                {
                    item.LoadSubKeys();
                }
            }
        }
    }
}