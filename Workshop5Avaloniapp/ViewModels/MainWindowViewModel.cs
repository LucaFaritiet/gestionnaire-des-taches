using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Workshop5Avaloniapp.Models;

namespace Workshop5Avaloniapp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private const double BytesToMegabytes = 1024.0 * 1024.0;

        [ObservableProperty]
        private ObservableCollection<ProcessInfo> _processes = new();

        [ObservableProperty]
        private ProcessInfo? _selectedProcess;

        public MainWindowViewModel()
        {
            RefreshProcesses();
        }

        [RelayCommand]
        private void RefreshProcesses()
        {
            Processes.Clear();
            var processes = Process.GetProcesses()
                .OrderBy(p => p.ProcessName)
                .ToList();

            foreach (var process in processes)
            {
                try
                {
                    var memoryMB = process.WorkingSet64 / BytesToMegabytes;
                    Processes.Add(new ProcessInfo
                    {
                        ProcessId = process.Id,
                        ProcessName = process.ProcessName,
                        MemoryUsage = $"{memoryMB:F2} MB"
                    });
                }
                catch
                {
                    // Some processes may throw access denied exceptions
                    Processes.Add(new ProcessInfo
                    {
                        ProcessId = process.Id,
                        ProcessName = process.ProcessName,
                        MemoryUsage = "N/A"
                    });
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanKillProcess))]
        private void KillProcess()
        {
            if (SelectedProcess == null) return;

            try
            {
                var process = Process.GetProcessById(SelectedProcess.ProcessId);
                process.Kill();
                RefreshProcesses();
            }
            catch
            {
                // Process may have already exited or access denied
            }
        }

        private bool CanKillProcess()
        {
            return SelectedProcess != null;
        }
    }
}
