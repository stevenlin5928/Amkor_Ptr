using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Windows;


namespace TagPrinterWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool isLock = false;
    public MainWindow()
    {
        InitializeComponent();

        Utility.startlog = true;

        try
        {
            Utility.LoadDataBaseConfig(".\\Config.txt");
            //App.SQLConnection = Utility.LoadDataBaseConfig(".\\dbConfig.txt");

        }
        catch(Exception ex)
        {
            Utility.Log($"Error: {ex.Message}");
        }

        ListBox_Log.ItemsSource = Utility.Loglist;

        System.Windows.Threading.DispatcherTimer logTimer = new System.Windows.Threading.DispatcherTimer();
        logTimer.Tick += new EventHandler(logTimer_Tick);
        logTimer.Interval = new TimeSpan(0, 0, 1);
        logTimer.Start();

        Task.Run(() =>
        {
            Utility u = new Utility();
            u.WriteLogFile($"Debug-{Utility.getDatetime4()}.log");
        });

    }

    private void logTimer_Tick(object? sender, EventArgs e)
    {
        if (isLock == false)
        {
            LoadLog();
        }
    }

    private void LoadLog()
    {
        isLock = true;
        while (true)
        {
            if (Utility._list.Count > 0)
            {
                Message m = Utility._list.Dequeue();
                Utility.Loglist.Add(m);

                if (Utility.Loglist.Count > 300)
                {
                    Utility.Loglist.Clear();
                }
            }
            else
            {
                break;
            }
        }
        isLock = false;
    }


    private void MM_PrintConfig_Click(object sender, RoutedEventArgs e)
    {
        Utility.Log($"SQL Connection: [{App.SQLConnection}]");
        Utility.Log($"Printer IP: [{App.PTR_IP}]");
        Utility.Log($"Printer Enable: [{App.enablePTR}]");
    }

    private void MM_Cls_Click(object sender, RoutedEventArgs e)
    {
        Utility.Loglist.Clear();
        //Utility.Log($"{302:X6}");
    }

    private void MM_Print_Click(object sender, RoutedEventArgs e)
    {

        Print _p = new Print();
        
        _p.Top = this.Top;
        _p.Left = this.Left;
        _p.ShowDialog();
    }


    private void MM_Tool_Click(object sender, RoutedEventArgs e)
    {
        //Utility.Log("Hello World!");
    }

    private void MM_MtrStockOut_Click(object sender, RoutedEventArgs e)
    {
        StockOutWin win = new StockOutWin();
        win.Top = this.Top + (this.Height - win.Height) / 2;
        win.WindowStyle = WindowStyle.ToolWindow;
        win.Left = this.Left + (this.Width - win.Width) / 2;
        win.Topmost = true;
        win.Show();
    }

    private void MM_StockIn_Click(object sender, RoutedEventArgs e)
    {
        StockIn win = new StockIn();
        win.Top = this.Top + (this.Height - win.Height) / 2;
        win.WindowStyle = WindowStyle.ToolWindow;
        win.Left = this.Left + (this.Width - win.Width) / 2;
        win.Topmost = true;
        win.Show();
    }

    private void MM_UploadInvData_Click(object sender, RoutedEventArgs e)
    {
        UploadInvData _p = new UploadInvData();
        _p.WindowStyle = WindowStyle.ToolWindow;
        _p.Top = this.Top;
        _p.Left = this.Left;
        _p.ShowDialog();
    }

    private void StartWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        Utility.startlog = false;
    }
}