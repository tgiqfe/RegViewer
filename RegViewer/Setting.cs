using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RegViewer
{
    internal class Setting
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int TreeWidth { get; set; }
        public int InfoHeight { get; set; }

        const string CONFIG_NAME = "Setting.json";

        public static Setting Load()
        {
            Setting setting = null;
            string path = Path.Combine(
                Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName), CONFIG_NAME);
            try
            {
                var json = File.ReadAllText(path);
                setting = JsonSerializer.Deserialize<Setting>(json);
            }
            catch { }
            if (setting == null)
            {
                setting = new()
                {
                    Left = 300,
                    Top = 300,
                    Width = 800,
                    Height = 450,
                    TreeWidth = 200,
                    InfoHeight = 120,
                };
                setting.Save();
            }

            return setting;
        }

        public void Save()
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName), CONFIG_NAME);
            try
            {
                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch { }
        }
    }
}
