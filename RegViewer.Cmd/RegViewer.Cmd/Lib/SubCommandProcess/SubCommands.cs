using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace RegViewer.Cmd.Lib.SubCommandProcess
{
    internal class SubCommands
    {
        protected virtual void SetupParameter(ArgsParam aParam) { }

        protected virtual bool CheckParameter() { return false; }

        public virtual void Run() { }

        public static SubCommands GetInstance(ArgsParam aParam)
        {
            string simpleClassName = aParam.SubCommand.ToString() + "Process";
            string fullClassName = $"RegViewer.Cmd.Lib.SubCommandProcess.{simpleClassName}";

            Type type = Type.GetType(fullClassName);
            if (type != null)
            {
                SubCommands instance = Activator.CreateInstance(type) as SubCommands;
                instance.SetupParameter(aParam);
                var ret = instance.CheckParameter();
                return ret ? instance : null;
            }
            return null;
        }
    }
}
