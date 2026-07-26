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

        public override void Run()
        {
            
        }
        
    }
}
