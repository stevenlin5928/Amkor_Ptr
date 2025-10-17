using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TagPrinterWPF
{
    /// <summary>
    /// UploadInvData.xaml 的互動邏輯
    /// </summary>
    public partial class UploadInvData : Window
    {
        ObservableCollection<daoInventoryStock> invDataSet = new ObservableCollection<daoInventoryStock>();
        string serialno = "";
        string[] EPC_Data;

        public UploadInvData()
        {
            InitializeComponent();

            Btn_Upload.Background = Brushes.Bisque;
            
            //綁定資料
            Listview_InvData.ItemsSource = invDataSet;

            Lbl_UpdateTime.Content = "";
            login();

            if (App.login_user == "")
            {

                Lbl_Empid.Foreground = new SolidColorBrush(Colors.Red);
                Lbl_Empid.Content = $"Not logged in!";
                Btn_Load.IsEnabled = false;
                Btn_Upload.IsEnabled = false;
                return;
            }


        }

        private void Btn_Load_Click(object sender, RoutedEventArgs e)
        {
            EPC_Data = new string[0];
            

            Task.Run(() =>
            {
                Load();
            });
        }

        private void Btn_Upload_Click(object sender, RoutedEventArgs e)
        {
            if(App.login_user == "")
            {
                Console.Beep(3500,500);
                MessageBox.Show("ERROR, 必須先登入才可以執行上傳盤點資料！");
                return;
            }

            //
            if(EPC_Data.Length == 0)
            {
                Console.Beep(3500, 500);
                MessageBox.Show("ERROR, 沒有盤點資料可以上傳！");
                return;
            }

            //
            //UPDATE TO AMS
            //
            AmkorMRSServices amkor = new AmkorMRSServices();
            bool result = amkor.SOAP_AMS_UpdSIDStocktaking(EPC_Data);
            //
            if (result == true)
            {
                objInventoryStock _myStock = new objInventoryStock();
                bool r = _myStock.setInventoryStockEnable(serialno, 1, App.login_user);
                if (r == true)
                {
                    Lbl_UpdateTime.Content = "";
                    invDataSet.Clear();
                    serialno = "";
                    Task.Run(() =>
                    {
                        Load();
                    });
                }
            }
            else
            {
                MessageBox.Show("ERROR, 盤點資料上傳失敗！");
            }
        }

        private void Load()
        {
            objInventoryStock _myStock = new objInventoryStock();
            Queue<daoInventoryStock> _q = _myStock.getInventoryStock();
            

            if (_q.Count == 0)
            {
                Console.Beep();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Lbl_UpdateTime.Content = "沒有可以上傳的盤點資料！";
                });
                return;
            }
            int count = 0;
            //string serial = "";

            int data_length = _q.Count;
            EPC_Data = new string[data_length];

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (daoInventoryStock item in _q)
                {
                    serialno = item.SerialNo;
                    invDataSet.Add(item);

                    //load to array
                    EPC_Data[count] = $"{item.TagID}";
                    count++;
                }
                Lbl_UpdateTime.Content = $" {serialno}  -- Total: {count}";
            });
            
        }


        public void login()
        {
            string empid = "";
            string opid = Interaction.InputBox("PLEASE INPUT OP ID：", "OP ID", "");
            if(opid == "")
            {
                return;
            }

            AmkorMRSServices _myService = new AmkorMRSServices();
            bool result = _myService.SOAP_CheckOpPermissions(opid, out empid);

            if(result == true)
            {
                App.login_user = empid;
                Lbl_Empid.Foreground = new SolidColorBrush(Colors.Blue);
                Lbl_Empid.Content = $"EmpID: {App.login_user}";
            }
            else
            {
                Console.Beep(3500, 500);
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.login_user = "";
        }
    }
}
