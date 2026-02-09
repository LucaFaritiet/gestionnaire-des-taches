using System.Diagnostics;

namespace Workshop5Avaloniapp.ViewModels;

public class ListProcessViewModel
{
    public Process[] Processes { get; } =  Process.GetProcesses();
}