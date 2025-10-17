using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagPrinterWPF
{
    class Message
    {
        private string _datetime;
        private string _log_message;

        public string DateTime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }

        public string LogMessage
        {
            get { return string.Format("[{0}]: {1}", _datetime, _log_message); }
            set { _log_message = value; }
        }

        private string _ProcMessage;
        private string _ProcMessageColor;

        public string ProcMessage
        {
            get { return string.Format("{0}", _ProcMessage); }
            set { _ProcMessage = value; }
        }

        public string ProcMessageColor
        {
            get { return _ProcMessageColor; }
            set { _ProcMessageColor = value; }
        }

        private string _ProcExInfo;
        public string ProcExInfo
        {
            get { return _ProcExInfo; }
            set { _ProcExInfo = value; }
        }

    }
}
