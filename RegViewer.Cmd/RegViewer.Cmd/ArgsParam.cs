using Microsoft.Win32;
using RegViewer.Cmd.Lib.RegistryCodes;
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
        public string ValueData { get; private set; }
        public bool IsDefaultValue { get; private set; }
        public RegistryValueKind ValueKind { get; private set; }
        public bool IsKey { get; private set; }
        public bool IsForce { get; private set; }

        public List<string> TextOptions { get; private set; } = new();

        public ArgsParam(string[] args)
        {
            if (args.Length > 1)
            {
                this.SubCommand = args[0].ToLower().Trim() switch
                {
                    "list" => SubCommand.List,
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
                switch (args[i].ToLower().Trim())
                {
                    case "/v":
                    case "-v":
                        if (args.Length > i + 1) this.ValueName = args[++i];
                        break;
                    case "/ve":
                    case "-ve":
                        this.ValueName = "";
                        break;
                    case "/d":
                    case "-d":
                        if (args.Length > i + 1) this.ValueData = args[++i];
                        break;
                    case "/t":
                    case "-t":
                        if (args.Length > i + 1) this.ValueKind = RegistryHelper.StringToValueKind(args[++i]);
                        break;
                    case "/f":
                    case "-f":
                        this.IsForce = true;
                        break;
                    case "/k":
                    case "-k":
                        this.IsKey = true;
                        break;
                    default:
                        this.TextOptions.Add(args[i]);
                        break;
                }
            }

            this.KeyPath = TextOptions.Count() == 0 ?
                null :
                TextOptions[0];
        }
    }
}
