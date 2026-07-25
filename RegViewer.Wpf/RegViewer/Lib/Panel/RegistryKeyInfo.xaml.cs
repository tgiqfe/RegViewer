using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// RegistryKeyInfo.xaml の相互作用ロジック
    /// </summary>
    public partial class RegistryKeyInfo : UserControl
    {
        public RegistryKeyInfo()
        {
            InitializeComponent();
        }

        private void GridSplitter_InfoPanel_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var splitter = sender as GridSplitter;
            if (splitter != null)
            {
                var grid = splitter.Parent as Grid;
                if (grid != null && grid.ColumnDefinitions.Count > 0)
                {
                    var width = grid.ColumnDefinitions[0].ActualWidth;
                    Item.BindingParam.Setting.InfoWidth = (int)width;
                }
            }
        }

        /// <summary>
        /// 継承されたアクセス許可の表示切り替えボタンがチェックされたときの処理
        /// (Onにしたとき)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ViewInheritedToggle_Checked(object sender, RoutedEventArgs e)
        {
            Item.BindingParam.KeyInformation.UpdateAclList(true);
        }

        /// <summary>
        /// 継承されたアクセス許可の表示切り替えボタンがチェック解除されたときの処理
        /// (Offにしたとき)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ViewInheritedToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Item.BindingParam.KeyInformation.UpdateAclList(false);
        }
    }
}
