
using RegViewer.Cmd;
using RegViewer.Cmd.Lib.SubCommandProcess;

var ap = new ArgsParam(args);

#if DEBUG



#endif

var sc = SubCommands.GetInstance(ap);




#if DEBUG
Console.ReadLine();
#endif
