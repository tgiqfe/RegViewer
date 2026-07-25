using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegViewer.Cmd
{
    internal class ArgsParam
    {
        public SubCommand SubCommand { get; private set; }
        public string KeyPath { get; private set; }
        public string ValueName { get; private set; }
        public bool IsDefaultValue { get; private set; }
        public RegistryValueKind ValueKind { get; private set; }

        public ArgsParam(string[] args)
        {
            if (args.Length > 1)
            {
                this.SubCommand = args[0].ToLower() switch
                {
                    "list" => SubCommand.List,
                    "keylist" => SubCommand.KeyList,
                    "get" => SubCommand.Get,
                    "set" => SubCommand.Set,
                    "delete" => SubCommand.Delete,
                    "copy" => SubCommand.Copy,
                    "move" => SubCommand.Move,
                    "export" => SubCommand.Export,
                    "search" => SubCommand.Search,
                    "grant" => SubCommand.Grant,
                    "revoke" => SubCommand.Revoke,
                    "load" => SubCommand.Load,
                    "unload" => SubCommand.Unload,
                    _ => throw new ArgumentException($"Unknown subcommand: {args[0]}")
                };
            }

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {

                }
            }
        }
    }
}
