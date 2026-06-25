using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RegViewer.Lib.Panel
{
    /// <summary>
    /// RegistryTree.xaml の相互作用ロジック
    /// </summary>
    public partial class RegistryTree : UserControl
    {
        private string _searchString = "";
        private DateTime _lastKeyPressTime = DateTime.MinValue;
        private const int SearchTimeoutMs = 1000;

        private KeyItem _selectedKeyItem = null;

        public RegistryTree()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is KeyItem selectedKeyItem)
            {
                _selectedKeyItem = selectedKeyItem;
                Item.BindingParam.AddressBar.Text = selectedKeyItem.Path;
                foreach (var item in selectedKeyItem.SubKeys)
                {
                    item.LoadSubKeys();
                }
            }
        }

        private void TreeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickedElement = e.OriginalSource as DependencyObject;
            if (clickedElement != null)
            {
                var treeViewItem = FindParent<TreeViewItem>(clickedElement);
                if (treeViewItem == null && _selectedKeyItem != null)
                {
                    SelectTreeViewItem(TreeView, _selectedKeyItem);
                    e.Handled = true;
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            if (parentObject is T parent)
                return parent;

            return FindParent<T>(parentObject);
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
            // 実際に展開されたアイテムのみを処理（バブリングを防ぐ/親アイテムの展開イベントを無視）
            if (e.OriginalSource == sender &&
                sender is TreeViewItem treeViewItem &&
                treeViewItem.DataContext is KeyItem expandingKeyItem)
            {
                if (!treeViewItem.IsSelected)
                {
                    treeViewItem.IsSelected = true;
                }
            }
        }

        private void TreeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TreeView treeView) return;

            // 文字キーのみを処理
            if ((e.Key >= Key.A && e.Key <= Key.Z) ||
                (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                e.Key == Key.Space ||
                e.Key == Key.OemPeriod ||
                e.Key == Key.OemComma)
            {
                var currentTime = DateTime.Now;

                // タイムアウトをチェック（1秒以内なら文字列を継続、それ以外はリセット）
                if ((currentTime - _lastKeyPressTime).TotalMilliseconds > SearchTimeoutMs)
                {
                    _searchString = "";
                }

                _lastKeyPressTime = currentTime;
                _searchString += GetCharFromKey(e.Key);

                // 現在表示されている項目から検索を開始
                var currentItem = treeView.SelectedItem as KeyItem;
                KeyItem foundItem = null;

                if (currentItem.Parent == null)
                {
                    foundItem = SearchInTreeView(treeView);
                }
                else
                {
                    var index = currentItem.Parent.SubKeys.IndexOf(currentItem);
                    foreach (var item in currentItem.Parent.SubKeys.Skip(index))
                    {
                        foundItem = SearchInKeyItem(item);
                        if (foundItem != null)
                        {
                            break;
                        }
                    }
                }

                // 見つかった項目を選択
                if (foundItem != null)
                {
                    SelectTreeViewItem(treeView, foundItem);
                }

                e.Handled = true;
            }
        }

        private char GetCharFromKey(Key key)
        {
            // キーコードを文字に変換する
            if (key >= Key.A && key <= Key.Z)
            {
                return (char)('a' + (key - Key.A));
            }
            else if (key >= Key.D0 && key <= Key.D9)
            {
                return (char)('0' + (key - Key.D0));
            }
            else if (key == Key.Space)
            {
                return ' ';
            }
            else if (key == Key.OemPeriod)
            {
                return '.';
            }
            else if (key == Key.OemComma)
            {
                return ',';
            }
            return '\0'; // 無効なキーの場合はヌル文字を返す
        }

        private KeyItem SearchInTreeView(TreeView treeView)
        {
            if (treeView.ItemsSource == null) return null;

            foreach (KeyItem rootItem in treeView.ItemsSource)
            {
                var result = SearchInKeyItem(rootItem);
                if (result != null) return result;
            }

            return null;
        }

        private KeyItem SearchInKeyItem(KeyItem item)
        {
            // 現在の項目が検索文字列で始まるかチェック
            if (item.Name != null && item.Name.ToLower().StartsWith(_searchString))
            {
                return item;
            }

            // 展開されている場合のみ、子項目を検索
            if (item.IsExpanded && item.SubKeys != null)
            {
                foreach (var subItem in item.SubKeys)
                {
                    var result = SearchInKeyItem(subItem);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        private void SelectTreeViewItem(TreeView treeView, KeyItem item)
        {
            TreeViewItem getTreeViewItem(ItemsControl container, object item)
            {
                if (container != null)
                {
                    if (container.DataContext == item)
                    {
                        return container as TreeViewItem;
                    }

                    // すべての子アイテムを確認
                    for (int i = 0; i < container.Items.Count; i++)
                    {
                        TreeViewItem subContainer = container.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
                        if (subContainer != null)
                        {
                            if (subContainer.DataContext == item)
                            {
                                return subContainer;
                            }

                            // 再帰的に子要素を検索
                            TreeViewItem resultContainer = getTreeViewItem(subContainer, item);
                            if (resultContainer != null)
                            {
                                return resultContainer;
                            }
                        }
                    }
                }
                return null;
            }

            // TreeViewItemを取得して選択
            var container = getTreeViewItem(treeView, item);
            if (container != null)
            {
                container.IsSelected = true;
                container.Focus();
                container.BringIntoView();
            }
        }
    }
}
