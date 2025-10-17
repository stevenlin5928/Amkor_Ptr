using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniPRT.Sdk.Comm;

namespace TagPrinterWPF
{
    class UtilityPrt
    {
        static TcpConnection PtrTcpComm;

        public static bool ConnectToPrt(string ipAddress)
        {
            Utility.Log($"Printer IP: [{App.PTR_IP}]");

            if (App.enablePTR == false) {
                Utility.Log($"App.enablePTR: {App.enablePTR}");
                return true;
            }

            if (PtrTcpComm != null && PtrTcpComm.Connected == true)
            {
                Utility.Log($"不需要連線： PtrTcpComm.Connected: {PtrTcpComm.Connected}");
                return true;
            }

            bool rsp = false;

            PtrTcpComm = new TcpConnection(ipAddress, TcpConnection.DEFAULT_DATA_PORT); // sending through default data port 9100
            try
            {
                PtrTcpComm.Open();
                rsp = PtrTcpComm.Connected;
                Utility.Log($"connecting to printer: {PtrTcpComm.Connected}");
            }
            catch (Exception ex)
            {
                Utility.Log($"Error connecting to printer: {ex.Message}");
            }

            return rsp;
        }

        public static void ClosePrt()
        {
            try
            {
                PtrTcpComm.Close();
            }
            catch (Exception ex)
            {
            }
            
        }

        public static bool SendPrintString(string SID, string TagID, string RequestNo, string opid)    // send print data over default printer data port
        {
            if (App.enablePTR == false)
            {
                Utility.Log($"App.enablePTR: {App.enablePTR}");
                return true;
            }

            bool rsp = false;

            int y = -40;
            int x = 420;

            //string data = "000000000101394340";
            //string tagid = "123456";
            string dataToPrint =
                $"~CREATE;C39;141\r\n" +
                //$"~FONT;FACE 93952\r\n" +
                $"SCALE;DOT;200;200\r\n" +

                $"RFWTAG;96;0;EPC\r\n" +
                $"96;H;*{SID + TagID}*\r\n" +
                $"STOP\r\n" +

                $"BARCODE\r\n" +
                //$"DATAMATRIX;INV;XD11;C24;R24;ECC200;{y + 80};{x + 60}\r\n" +
                $"DATAMATRIX;INV;XD11;C24;R24;ECC200;{y + 50};{x + 60}\r\n" +
                $"\"{SID}-{TagID}-{RequestNo}\"\r\n" +
                $"STOP\r\n" +

                $"ALPHA\r\n" +

                $"C14;D;{y + 260};{x};0;0;*RequestNo:{RequestNo}*\r\n" +
                $"C14;D;{y + 290};{x};0;0;*SID:{SID}*\r\n" +
                $"C14;D;{y + 320};{x};0;0;*TAGID:{TagID}*\r\n" +
                $"C14;D;{y + 350};{x};0;0;*OP ID:{opid}*\r\n" +
                $"C14;D;{y + 380};{x};0;0;*{Utility.getDatetime()}*\r\n" +

                $"STOP\r\n" +
                $"END\r\n" +
                $"~EXECUTE;C39\r\n" +
                $"~NORMAL\r\n\r\n";

            try
            {
                if (PtrTcpComm.Connected)
                {
                    Utility.Log($"PtrTcpComm.Write--->PtrTcpComm.Connected:{PtrTcpComm.Connected}");

                    byte[] outBytes = Encoding.ASCII.GetBytes(dataToPrint);
                    PtrTcpComm.Write(outBytes);
                    

                    rsp = true;
                }
                else
                {
                    Utility.Log($"Not connected to printer");
                }
            }
            catch (Exception e)
            {
                Utility.Log($"Exception Msg: {e}");
            }

            finally
            {

            }

            return rsp;
        }
    }
}
