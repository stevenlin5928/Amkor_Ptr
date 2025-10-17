using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using t3whcpapp01;
using static TagPrinterWPF.App;

namespace TagPrinterWPF
{
    class AmkorMRSServices
    {
        //AmkorServices1.AMSServiceForVendor AMS_Services = new AmkorServices1.AMSServiceForVendor();
        //t3whcpapp01.HRMWebService HRM_WebService = new t3whcpapp01.HRMWebService();
        string ProdLine = "DP";
        string Floor = "2F";

        public AmkorMRSServices()
        {
            
            

        }

        public bool SOAP_AMS_StockIn(object[] data)
        {
            string empid = App.login_user;
            bool r = false;

            if (data.Length == 0)
            {
                return false;
            }

            for(int index = 0; index < data.Length; index++)
            {
                Utility.Log($"--->{data[index]}");
            }

            try
            {
                var client = new AmkorServices1.AMSServiceForVendorSoapClient(AmkorServices1.AMSServiceForVendorSoapClient.EndpointConfiguration.AMSServiceForVendorSoap);
                var result = client.AMS_StockIn(data, empid, "UHF_StockIn");
                string AMSResult = result.ToString();

                Utility.Log($"AMS_Services.AMS_StockIn: {AMSResult} ,empid:{empid}");
                if (AMSResult.Contains("Success"))
                {
                    r = true;
                }

            }
            catch (Exception ex)
            {
                Utility.Log($"SOAP_AMS_StockIn : {ex}");
            }

            return r;

        }

        public bool SOAP_AMS_UpdSIDStocktaking(string[] data)
        {
            string empid = App.login_user;
            bool r = false;

            try
            {
                Utility.Log($"-->data length:{data.Length}, ProdLine:{ProdLine}, Floor:{Floor}, empid:{empid}");
                for (int index = 0; index < data.Length; index++) {
                    Utility.Log($"data: {data[index]}");
                }
                

                //$ProdLine, $Floor, $DataList, $EmpId
                var client = new AmkorServices1.AMSServiceForVendorSoapClient(AmkorServices1.AMSServiceForVendorSoapClient.EndpointConfiguration.AMSServiceForVendorSoap);
                var result = client.AMS_UpdSIDStocktaking(ProdLine, Floor, data, empid);

                Utility.Log($"AMS_UpdSIDStocktaking result: {result.ToString()}");

                if (result.ToString().ToUpper().Contains("OK") == true || result.ToString().ToUpper().Contains("SUCCESS"))
                {
                    Utility.Log($"AMS_UpdSIDStocktaking success!");
                    r = true;
                }
            }
            catch (Exception ex)
            {
                Utility.Log($"AMS_UpdSIDStocktaking : {ex}");
            }

            return r;
            
        }

        public bool SOAP_CheckOpPermissions(string cardID, out string empid)
        {
            string _empid = "";
            bool r = false;


            var client = new t3whcpapp01.HRMWebServiceSoapClient(HRMWebServiceSoapClient.EndpointConfiguration.HRMWebServiceSoap);
            var result =  client.HCPDBSQLQuery2Dataset("DPS_UHF", $"select code from hcp.xamk_temp_personnel_scan where subid like '%{cardID}'");
            foreach (XElement el in result.Nodes)
            {
                //Console.WriteLine(el.Name);
                foreach (var child in el.Elements())
                {
                    _empid = child.Value;
                    Utility.Log($"{child.Name}: ---->{child.Value}");
                }
            }
            
            App.login_user = _empid;
            empid = _empid;

            if (_empid == "")
            {
                return r;
            }

            var amkor = new AmkorServices1.AMSServiceForVendorSoapClient(AmkorServices1.AMSServiceForVendorSoapClient.EndpointConfiguration.AMSServiceForVendorSoap);
            var resp = amkor.AMS_isMaterialOP(_empid);
            if (resp.ToUpper().Contains("SUCCESS") == true)
            {
                r = true;
            }
            Utility.Log($"resp: ---->{resp}, return: {r}");

            return r;

        }

    }
}
