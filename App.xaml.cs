using System.IO;
using System.Windows;
using System.Windows.Threading;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace LTTPEnhancementTools;

public partial class App : Application
{
    private static readonly string CrashLogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "LTTPEnhancementTools", "crash.log");

    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException       += OnDispatcherException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainException;
        TaskScheduler.UnobservedTaskException      += OnUnobservedTask;
        base.OnStartup(e);
    }

    // ── UI-thread exceptions (WPF dispatcher) ─────────────────────────────
    private void OnDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogCrash(e.Exception);
        ShowCrashDialog(e.Exception);
        e.Handled = true; // keep the app alive so the user can see the dialog
    }

    // ── Any-thread unhandled exceptions ───────────────────────────────────
    private static void OnDomainException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
            LogCrash(ex);
    }

    // ── Unobserved Task exceptions ────────────────────────────────────────
    private static void OnUnobservedTask(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogCrash(e.Exception);
        e.SetObserved();
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    private static void LogCrash(Exception ex)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CrashLogPath)!);
            File.AppendAllText(CrashLogPath,
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\r\n{ex}\r\n\r\n");
        }
        catch { /* never crash inside the crash handler */ }
    }

    private static void ShowCrashDialog(Exception ex)
    {
        MessageBox.Show(
            $"An unexpected error occurred:\r\n\r\n{ex.Message}\r\n\r\n" +
            $"A crash log has been saved to:\r\n{CrashLogPath}\r\n\r\n" +
            "Please share this log when reporting the issue.",
            "ALttP MSU-1 Music Switcher — Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
