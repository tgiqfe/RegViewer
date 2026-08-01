using RegViewer.Cmd.Lib.RegistryCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegViewer.Cmd.Lib.SubCommandProcess
{
    internal class ListProcess : SubCommands
    {
        public string KeyPath { get; private set; }
        public bool IsKey { get; private set; }

        protected override void SetupParameter(ArgsParam aParam)
        {
            KeyPath = aParam.KeyPath;
            IsKey = aParam.IsKey;
        }

        protected override bool CheckParameter()
        {
            return !string.IsNullOrEmpty(KeyPath);
        }

        public override void Run()
        {
            using(var retKey = RegistryHelper.GetRegistryKey(this.KeyPath, false, false))
            {
                if (retKey != null)
                {
                    if (IsKey)
                    {
                        var subKeys = retKey.GetSubKeyNames();
                        foreach (var subKey in subKeys)
                        {
                            Console.WriteLine(subKey);
                        }
                    }
                    else
                    {
                        var valueNames = retKey.GetValueNames();
                        foreach (var valueName in valueNames)
                        {
                            Console.WriteLine(valueName);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"The specified registry path '{KeyPath}' does not exist.");
                }
            }
        }
    }
}
