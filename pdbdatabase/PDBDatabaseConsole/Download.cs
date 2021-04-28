//using System;
//using System.IO;
//using System.Net;
//using System.Threading;

//namespace Batte.CodeProject.Download
//{
//    public class FileDownloader : IDisposable
//    {
//        private WaitHandle cancelEvent;

//        public FileDownloader()
//        {
//            // make all WebRequests use the same Proxy info that IE uses
//            GlobalProxySelection.Select = WebProxy.GetDefaultProxy();
//            GlobalProxySelection.Select.Credentials = CredentialCache.DefaultCredentials;
//        }

//        public event DownloadProgressHandler ProgressChanged;
//        public event DownloadProgressHandler StateChanged;

//        private void RaiseStateChanged(string state)
//        {
//            if(this.StateChanged != null)
//                this.StateChanged(this, new DownloadEventArgs(state));
//        }
//        private void RaiseProgressChanged(long current, long target)
//        {
//            int percent = (int) ((((double) current) / target) * 100);
//            if(this.ProgressChanged != null)
//                this.ProgressChanged(this, new DownloadEventArgs(percent));
//        }
//        private bool HasUserCancelled()
//        {
//            return (this.cancelEvent != null && this.cancelEvent.WaitOne(0, false));
//        }
//        public void Dispose()
//        {
//            this.cancelEvent = null;
//        }
//    }
//}
