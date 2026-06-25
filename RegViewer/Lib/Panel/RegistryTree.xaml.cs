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
        private readonly List<KeyItem> _selectedItems = new List<KeyItem>();
        private KeyItem _lastSelectedItem = null;

        public RegistryTree()
        {
            InitializeComponent();
        }

        public IReadOnlyList<KeyItem> SelectedItems => _selectedItems.AsReadOnly();

        /// <summary>
        /// TreeViewの選択が変更されたときに呼び出されるイベントハンドラー
        /// 選択したアイテムの一つ下の階層のサブキーをロードする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is KeyItem selectedKeyItem)
            {
                _selectedKeyItem = selectedKeyItem;
                Item.BindingParam.AddressBar.Text = selectedKeyItem.Path;
                selectedKeyItem.LoadSubKeys();

                // 通常のTreeView選択が変更された場合、単一選択に更新
                // ただし、既に複数選択がある場合（Ctrl/Shift操作中）はスキップ
                if (_selectedItems.Count <= 1)
                {
                    ClearSelection();
                    _selectedItems.Add(selectedKeyItem);
                    selectedKeyItem.IsSelected = true;
                    _lastSelectedItem = selectedKeyItem;
                }
            }
        }

        /// <summary>
        /// TreeViewのPreviewMouseDownイベントハンドラー
        /// マウスのホイール上下でスクロールするために、TreeViewItem以外の部分をクリックした場合に選択を解除しないようにする
        /// 複数選択機能: Ctrl/Shiftキーとの組み合わせで複数選択を実現
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickedElement = e.OriginalSource as DependencyObject;
            if (clickedElement != null)
            {
                var treeViewItem = FindParent<TreeViewItem>(clickedElement);
                if (treeViewItem != null && treeViewItem.DataContext is KeyItem clickedItem)
                {
                    // 展開トグル（左側の三角形）をクリックした場合はスキップ
                    if (IsClickOnToggleButton(e.OriginalSource as FrameworkElement))
                    {
                        return;
                    }

                    bool isCtrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                    bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

                    if (isCtrlPressed)
                    {
                        // Ctrl: トグル選択
                        if (_selectedItems.Contains(clickedItem))
                        {
                            _selectedItems.Remove(clickedItem);
                            clickedItem.IsSelected = false;
                        }
                        else
                        {
                            _selectedItems.Add(clickedItem);
                            clickedItem.IsSelected = true;
                        }
                        _lastSelectedItem = clickedItem;
                        e.Handled = true;
                    }
                    else if (isShiftPressed && _lastSelectedItem != null)
                    {
                        // Shift: 範囲選択
                        ClearSelection();
                        SelectRange(_lastSelectedItem, clickedItem);
                        e.Handled = true;
                    }
                    else
                    {
                        // 通常クリック: 単一選択
                        ClearSelection();
                        _selectedItems.Add(clickedItem);
                        clickedItem.IsSelected = true;
                        _lastSelectedItem = clickedItem;
                        // 通常クリックの場合はTreeViewの標準動作を残すため、e.Handledは設定しない
                    }
                }
                else if (treeViewItem == null && _selectedKeyItem != null)
                {
                    SelectTreeViewItem(TreeView, _selectedKeyItem);
                    e.Handled = true;
                }
            }
        }

        private bool IsClickOnToggleButton(FrameworkElement element)
        {
            // 展開トグルボタンまたはその子要素をクリックしたかチェック
            while (element != null)
            {
                if (element is System.Windows.Controls.Primitives.ToggleButton)
                {
                    return true;
                }
                element = System.Windows.Media.VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
            return false;
        }

        private void ClearSelection()
        {
            foreach (var item in _selectedItems)
            {
                item.IsSelected = false;
            }
            _selectedItems.Clear();
        }

        private void SelectRange(KeyItem start, KeyItem end)
        {
            var allItems = GetAllVisibleItems(TreeView);
            int startIndex = allItems.IndexOf(start);
            int endIndex = allItems.IndexOf(end);

            if (startIndex != -1 && endIndex != -1)
            {
                int minIndex = Math.Min(startIndex, endIndex);
                int maxIndex = Math.Max(startIndex, endIndex);

                for (int i = minIndex; i <= maxIndex; i++)
                {
                    var item = allItems[i];
                    if (!_selectedItems.Contains(item))
                    {
                        _selectedItems.Add(item);
                        item.IsSelected = true;
                    }
                }
            }
        }

        private List<KeyItem> GetAllVisibleItems(TreeView treeView)
        {
            var result = new List<KeyItem>();
            if (treeView.ItemsSource != null)
            {
                foreach (KeyItem rootItem in treeView.ItemsSource)
                {
                    AddVisibleItems(rootItem, result);
                }
            }
            return result;
        }

        private void AddVisibleItems(KeyItem item, List<KeyItem> result)
        {
            result.Add(item);
            if (item.IsExpanded && item.SubKeys != null)
            {
                foreach (var subItem in item.SubKeys)
                {
                    AddVisibleItems(subItem, result);
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

        private async void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
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

                // 展開時に配下のサブキーを並列で先読みロード
                if (expandingKeyItem.SubKeys != null && expandingKeyItem.SubKeys.Count > 0)
                {
                    await Task.Run(() =>
                    {
                        Parallel.ForEach(expandingKeyItem.SubKeys, new ParallelOptions
                        {
                            MaxDegreeOfParallelism = Environment.ProcessorCount
                        }, item =>
                        {
                            item.LoadSubKeys();
                        });
                    });
                }
            }
        }

        private void TreeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TreeView treeView) return;

            // Shift+↑/↓で選択範囲を拡張
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && (e.Key == Key.Up || e.Key == Key.Down))
            {
                HandleShiftArrowKey(e.Key == Key.Up);
                e.Handled = true;
                return;
            }

            // Shiftなし↑/↓で単一選択に戻す
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                ClearSelection();
                // TreeViewの標準動作に任せるため、e.Handledは設定しない
                // SelectedItemChangedイベントで新しい選択が処理される
                return;
            }

            if (e.Key == Key.Delete)
            {
                MessageBox.Show(string.Join(", ", _selectedItems.Select(x => x.Path)));
            }

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

        private void HandleShiftArrowKey(bool isUp)
        {
            var allItems = GetAllVisibleItems(TreeView);
            if (allItems.Count == 0) return;

            // 最初の選択がない場合は、最初のアイテムを選択
            if (_selectedItems.Count == 0)
            {
                var firstItem = allItems[0];
                _selectedItems.Add(firstItem);
                firstItem.IsSelected = true;
                _lastSelectedItem = firstItem;
                return;
            }

            // 現在フォーカスのあるアイテム（TreeViewの選択アイテム）を取得
            var currentItem = TreeView.SelectedItem as KeyItem;
            if (currentItem == null && _lastSelectedItem != null)
            {
                currentItem = _lastSelectedItem;
            }
            if (currentItem == null) return;

            int currentIndex = allItems.IndexOf(currentItem);
            if (currentIndex == -1) return;

            // 移動先のインデックスを計算
            int targetIndex = isUp ? currentIndex - 1 : currentIndex + 1;
            if (targetIndex < 0 || targetIndex >= allItems.Count) return;

            var targetItem = allItems[targetIndex];

            // アンカーポイント（選択の起点）を決定
            if (_lastSelectedItem == null)
            {
                _lastSelectedItem = currentItem;
            }

            // 選択範囲を再計算
            ClearSelection();
            SelectRange(_lastSelectedItem, targetItem);

            // TreeViewのフォーカスを移動（スクロールのため）
            SelectTreeViewItem(TreeView, targetItem);
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
