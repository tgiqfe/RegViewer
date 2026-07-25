using System;
using System.Collections.Generic;
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
    public partial class RegistryAddressBar : UserControl
    {
        public RegistryAddressBar()
        {
            InitializeComponent();
        }

        private void AddressBar_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    //  AddressBarに入力されたパスへ移動
                    string path = AddressBar.Text;
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        path = path.Trim();
                        
                        var mainWindow = Application.Current.MainWindow as MainWindow;
                        
                        
                    }
                    e.Handled = true;
                    break;
            }
        }
    }
}
