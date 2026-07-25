using Microsoft.Win32;
using RegViewer.Lib.RegistryCodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RegViewer.Lib.Panel
{
    public partial class ItemEditWindow : Window
    {
        public string KeyPath { get; set; }
        public string ParamName { get; set; }
        public object ParamValue { get; set; }
        public RegistryValueKind ParamType { get; set; }

        public KeyItem KeyItem { get; set; }

        public enum EditAction
        {
            AddKey,
            AddValue,
            DeleteKey,
            DeleteValue,
            RenameKey,
            RenameValue,
            CopyKey,
            CopyValue,
            PasteKey,
            PasteValue,
        }


        public EditAction Action { get; set; }

        public ItemEditWindow(KeyItem keyItem)
        {
            this.DataContext = Item.BindingParam;
            InitializeComponent();

            this.KeyPath = keyItem.Path;
            this.KeyItem = keyItem;
        }



        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is Button) && !(e.Source is TextBox))
            {
                this.DragMove();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Close();
                    Item.IsViewEditWindow = false;
                    e.Handled = true;
                    break;
                case Key.Enter:
                    ButtonOK_Click(sender, e);
                    break;
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (this.Action)
                {
                    case EditAction.AddKey:
                        AddKeyAction();
                        break;
                }
            }
            catch
            {
                MessageBox.Show("編集失敗", "Registry edit error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Item.IsViewEditWindow = false;
        }

        private void AddKeyAction()
        {
            string newKeyName = Path.Combine(this.KeyPath, NewInput_Key.Text);

            bool isKeyExists = false;
            using (var regKey = RegistryHelper.GetRegistryKey(newKeyName, false, false))
            {
                if (regKey != null)
                {
                    isKeyExists = true;
                }
            }
            if (isKeyExists)
            {
                EditErrorMessage.Text = "キーは既に存在します。";
                return;
            }

            //  Create the new key
            using (var regKey = RegistryHelper.GetRegistryKey(newKeyName, true, true))
            {
            }
            this.KeyItem.RenewSubKeys();

            this.Close();
            Item.IsViewEditWindow = false;
        }
    }
}
