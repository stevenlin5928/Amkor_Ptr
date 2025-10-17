using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TagPrinterWPF
{
    /// <summary>
    /// StockIn.xaml 的互動邏輯
    /// </summary>
    public partial class StockIn : Window
    {
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        int timeout = 30;
        int timeCount = 30;
        private ObservableCollection<daoWhiteList> _WhiteList = new ObservableCollection<daoWhiteList>();
        public StockIn()
        {
            InitializeComponent();
            
            Lbl_UpdateMessage.Content = "";

            login();
            if (App.login_user == "")
            {
                
                Lbl_Empid.Foreground = new SolidColorBrush(Colors.Red);
                Lbl_Empid.Content = $"Not logged in!";
                Btn_Load.IsEnabled = false;
                Btn_Upload.IsEnabled = false;
                return;
            }


            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 10);

            ListView_Tags.ItemsSource = _WhiteList;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (timeout > 0)
            {
                timeout--;

            }
            else
            {
                App.login_user = "";
                Lbl_Empid.Foreground = new SolidColorBrush(Colors.Red);
                Lbl_Empid.Content = $"Not logged in!";

                //timeout = timeCount;
            }
        }

        public void login()
        {

            
            string empid = "";
            string opid = Interaction.InputBox("PLEASE INPUT OP ID：", "OP ID", "");
            if (opid == "")
            {
                Console.Beep(3500, 500);
                return;
            }
            AmkorMRSServices _myService = new AmkorMRSServices();
            bool result = _myService.SOAP_CheckOpPermissions(opid, out empid);

            if (result == true)
            {
                App.login_user = empid;
                Lbl_Empid.Foreground = new SolidColorBrush(Colors.Blue);
                Lbl_Empid.Content = $"EmpID: {App.login_user}";
            }
            else
            {
                Console.Beep(3500, 500);
            }



            //timeout = timeCount;

            //Timer.Start();
        }

        private void Btn_Load_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Load(true);
            });

        }

        private void Btn_Upload_Click(object sender, RoutedEventArgs e)
        {
            if(App.login_user == "")
            {
                Console.Beep(3500, 500);
                MessageBox.Show("需要新登入，才可以上傳資料！");
                return;
            }

            List<daoWhiteList> _wl = new List<daoWhiteList>();
            objWhiteList _objlist = new objWhiteList();

            string[] data = new string[_WhiteList.Count];
            int index = 0;
            foreach (daoWhiteList item in _WhiteList)
            {
                Utility.Log($"TagID: {item.TagID}, isClecked: {item.Checked}");

                if(item.Checked == true)
                {
                    //prodline, floor, requestNo, SID, Vendor Lot, ReEPC, remark
                    //string _s = "DP,2F,C001367377,000000006001008029,1234567890,FF000000006001008029,DP2F";
                    string _s = $"DP,2F,{item.RequestNo},{item.SID},,{item.SID}{item.TagID},DP2F";
                    data[index++] = _s;

                    daoWhiteList daoWl = new daoWhiteList();
                    daoWl.RequestNo = item.RequestNo;
                    daoWl.SID = item.SID;
                    daoWl.TagID = item.TagID;
                    _wl.Add(daoWl);
                }
                
            }
            object[] resized = data.Where(o => o != null).ToArray();
            AmkorMRSServices amkor = new AmkorMRSServices();
            bool result = amkor.SOAP_AMS_StockIn(resized);
            if(result == false)
            {
                Console.Beep(3500, 500);
                MessageBox.Show("ERROR：上傳物料失敗！");
                return;
            }
            else
            {

                _objlist.SetStockEnable(_wl, 2, App.login_user);
                Task.Run(() =>
                {
                    Load(true);
                });
            }
            
        }

        public void Load(bool _check)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _WhiteList.Clear();
            });

            objWhiteList tmp = new objWhiteList();
            List<daoWhiteList> myWL = tmp.Load2(1);
            if (myWL.Count == 0)
            {
                Console.Beep(3500, 500);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Lbl_UpdateMessage.Content = "沒有物料可以上傳AMS！";
                });

                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (daoWhiteList wl in myWL)
                {
                    wl.Checked = _check;
                    _WhiteList.Add(wl);
                }

                Lbl_UpdateMessage.Content = $"一共有 {myWL.Count} 筆可以上傳！";
            });
        }

        private void CleckBox_SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            
              
        }

        private void CleckBox_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            bool c = false;

            if (CleckBox_SelectAll.IsChecked == true)
            {
                c = true;
            }

            Load(c);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.login_user = "";
        }
    }
}
