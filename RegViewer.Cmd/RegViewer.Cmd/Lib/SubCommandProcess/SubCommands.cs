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

        public virtual void Run() { }

        public static SubCommands GetInstance(string subCommand, ArgsParam aParam)
        {
            string simpleClassName = char.ToUpper(subCommand[0]) + subCommand.Substring(1).ToLower() + "Process";
            string fullClassName = $"RegViewer.Cmd.Lib.SubCommandProcess.{simpleClassName}";

            Type type = Type.GetType(fullClassName);
            if (type != null)
            {
                SubCommands instance = Activator.CreateInstance(type) as SubCommands;
                instance.SetupParameter(aParam);
                return instance;
            }
            return null;
        }
    }
}
