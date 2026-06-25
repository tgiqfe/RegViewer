using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public static readonly DependencyProperty KeyItemsProperty =
            DependencyProperty.Register(
                nameof(KeyItems),
                typeof(ObservableCollection<KeyItem>),
                typeof(RegistryTree),
                new PropertyMetadata(null));

        public ObservableCollection<KeyItem> KeyItems
        {
            get => (ObservableCollection<KeyItem>)GetValue(KeyItemsProperty);
            set => SetValue(KeyItemsProperty, value);
        }

        public RegistryTree()
        {
            InitializeComponent();
        }



        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
    
        }

        private void TreeView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
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

        #region TextSearch

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
                _searchString += e.Key.ToString().ToLower();

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



                if (currentItem != null && currentItem.SubKeys != null)
                {
                    // 現在選択されている項目の兄弟要素から検索
                    //foundItem = SearchInSiblings(treeView, currentItem);
                }

                // 兄弟要素で見つからなければ、ツリー全体から検索
                if (foundItem == null)
                {
                    //foundItem = SearchInTreeView(treeView);
                }

                // 見つかった項目を選択
                if (foundItem != null)
                {
                    SelectTreeViewItem(treeView, foundItem);
                }

                e.Handled = true;
            }
        }



        private KeyItem SearchInSiblings(TreeView treeView, KeyItem currentItem)
        {
            // 親の取得が難しいので、ルートから検索する方が確実
            return null;
        }

        private KeyItem SearchInTreeView(TreeView treeView)
        {
            if (treeView.ItemsSource == null)
                return null;

            foreach (KeyItem rootItem in treeView.ItemsSource)
            {
                var result = SearchInKeyItem(rootItem);
                if (result != null)
                    return result;
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
            // TreeViewItemを取得して選択
            var container = GetTreeViewItem(treeView, item);
            if (container != null)
            {
                container.IsSelected = true;
                container.BringIntoView();
            }
        }

        private TreeViewItem GetTreeViewItem(ItemsControl container, object item)
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
                        TreeViewItem resultContainer = GetTreeViewItem(subContainer, item);
                        if (resultContainer != null)
                        {
                            return resultContainer;
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
