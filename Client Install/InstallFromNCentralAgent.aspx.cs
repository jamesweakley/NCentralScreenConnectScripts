using System.Diagnostics;
using System.Web.Services;
using System.ComponentModel;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Data;
using System.Text;
using System.Configuration;
using Elsinore.ScreenConnect;

namespace NCentral
{
    public partial class DownloadScreenConnect : Elsinore.ScreenConnect.ThemeablePage
    {
        protected override void OnLoad(EventArgs e)
	    {
	        string applianceIDString = Request.QueryString.Get("ApplianceID");
            if (applianceIDString == null)
            {
                throw new Exception("ApplianceID not supplied");
            }
            long applianceID;
            if (!long.TryParse(applianceIDString, out applianceID))
            {
                throw new Exception("ApplianceID " + applianceIDString + " not a number");
            }
            string keyString = Request.QueryString.Get("Key");
            if (keyString == null)
            {
                throw new Exception("Key not supplied");
            }
            String serverKey = HttpServerUtility.UrlTokenEncode(ServerCryptoManager.Instance.PublicKey);
            if (!serverKey.Substring(0, 10).Equals(keyString))
            {
                throw new Exception("Key parameter must be the first 10 characters of the ScreenConnect server key");
            }

            NCentralMachineFacade nmf = new NCentralMachineFacade();
            DeviceInfo result = nmf.getDeviceInfo(getParameter("NCentralServerAddress") + "/dms/services/ServerEI",
                                                    getParameter("NCentralAPIUsername"),
                                                    getParameter("NCentralAPIPassword"),
                                                    applianceIDString);
            string customerName = null;
            foreach (T_KeyValue i in result.Info)
            {
                if (i.Key.Equals("device.customername"))
                {
                    customerName = i.Value[0];
                    break;
                }
            }
            if (customerName == null)
            {
                throw new Exception("N-Central API call to getDeviceInfo for applianceID "+applianceIDString+" did not return a value for device.customername");
            }
            var relayUri = ServerExtensions.GetRelayUri(ConfigurationManager.AppSettings, HttpContext.Current.Request.Url, true, true);

            string[] propertyValues=new string[1];
            propertyValues[0] = customerName;
            var clientLaunchParameters = new ClientLaunchParameters
            {
                SessionType = SessionType.Access,
                ProcessType = ProcessType.Guest,
                NameCallbackFormat = "",
                CustomPropertyValueCallbackFormats = propertyValues,
                Host = relayUri.Host,
                Port = relayUri.Port,
                EncryptionKey = ServerCryptoManager.Instance.PublicKey,
            };
                
            var wildcardedHandlerPath = WebConfigurationManager.GetHandlerPath(typeof(InstallerHandler));
            var handlerPath = ServerExtensions.RegexReplace(wildcardedHandlerPath, "\\*", (m, i) => i == 0 ? Resources.Default.FileDownload_BaseName : "msi");
            handlerPath = handlerPath + ClientLaunchParameters.ToQueryString(clientLaunchParameters);

            Response.Redirect(getParameter("WebServerAddressableUri") + handlerPath);
            Response.End();
	    }

        private static string getParameter(string parameterName)
        {
            string value = ConfigurationManager.AppSettings[parameterName];
            if (value == null)
            {
                throw new Exception("Please define parameter '" + parameterName + "' in <appSettings> in web.config");
            }
            return value;
        }
    }

    public class NCentralMachineFacade
    {
        /// <summary>
        /// Fetches Device info from the N-Central server via its SOAP API
        /// </summary>
        /// <param name="applianceID"></param>
        /// <returns></returns>
        public DeviceInfo getDeviceInfo(String serverUrl, String username, String password, String applianceID)
        {
            if (applianceID == null || applianceID.Length == 0)
            {
                throw new Exception("Empty ApplianceID");
            }
            ServerEIService s = new ServerEIService(serverUrl);

            T_KeyPair[] keyPairs = new T_KeyPair[1];
            T_KeyPair deviceID = new T_KeyPair();
            deviceID.Key = "applianceID";
            deviceID.Value = applianceID;
            keyPairs[0] = deviceID;

            DeviceInfo[] results = null;
            DeviceInfo result = null;
            results = s.DeviceGet(username, password, keyPairs);
            if (results.Length == 0)
            {
                throw new Exception("N-Central API call to getDeviceInfo returned no results for Appliance ID " + applianceID);
            }
            result = results[0];
            
            return result;
        }

    
    }

    /**
     * Everything below here was automatically generated by visual studio from the N-Central API WSDL
     * 
     */

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ServerEISoapBinding", Namespace = "http://www.n-able.com/mickey")]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(T_Pair))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(JobStatus))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(DeviceStatus))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(T_KeyValue))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(DeviceInfo))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(Customer))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(DeviceReportDeviceData))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(DeviceReportData))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(DeviceIssue))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(T_KeyPair))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(Device))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(DeviceHardDriveData))]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(DeviceData))]
    public partial class ServerEIService : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback RepositoryItemListOperationCompleted;

        private System.Threading.SendOrPostCallback RepositoryItemGetOperationCompleted;

        private System.Threading.SendOrPostCallback RepositoryItemPublishOperationCompleted;

        private System.Threading.SendOrPostCallback AcknowledgeNotificationOperationCompleted;

        private System.Threading.SendOrPostCallback DeviceAssetInfoExportOperationCompleted;

        private System.Threading.SendOrPostCallback DeviceAssetInfoExport2OperationCompleted;

        private System.Threading.SendOrPostCallback ActiveIssuesListOperationCompleted;

        private System.Threading.SendOrPostCallback PSACredentialsValidateOperationCompleted;

        private System.Threading.SendOrPostCallback CustomerDeviceReportOperationCompleted;

        private System.Threading.SendOrPostCallback CustomerDeviceReportGetOperationCompleted;

        private System.Threading.SendOrPostCallback CustomerListOperationCompleted;

        private System.Threading.SendOrPostCallback CustomerListChildrenOperationCompleted;

        private System.Threading.SendOrPostCallback DeviceGetOperationCompleted;

        private System.Threading.SendOrPostCallback DeviceGetStatusOperationCompleted;

        private System.Threading.SendOrPostCallback DeviceListOperationCompleted;

        private System.Threading.SendOrPostCallback SOAddOperationCompleted;

        private System.Threading.SendOrPostCallback CustomerAddOperationCompleted;

        private System.Threading.SendOrPostCallback UserAddOperationCompleted;

        private System.Threading.SendOrPostCallback JobStatusListOperationCompleted;

        private System.Threading.SendOrPostCallback LastExportResetOperationCompleted;

        private System.Threading.SendOrPostCallback LastExportHardResetOperationCompleted;

        private System.Threading.SendOrPostCallback SystemSetAuditorSettingsOperationCompleted;

        private System.Threading.SendOrPostCallback SystemSetAuditorSettings3OperationCompleted;

        private System.Threading.SendOrPostCallback SystemGetAuditorSettingsOperationCompleted;

        private System.Threading.SendOrPostCallback SystemVerifyAuditorSettingsOperationCompleted;

        private System.Threading.SendOrPostCallback SystemScheduleDataPushOperationCompleted;

        private System.Threading.SendOrPostCallback SystemGetUserOperationCompleted;

        private System.Threading.SendOrPostCallback SystemSetMonitoringTasksOperationCompleted;

        private System.Threading.SendOrPostCallback SystemGetSessionIDOperationCompleted;

        /// <remarks/>
        public ServerEIService(String url)
        {
            this.Url = url;
        }

        /// <remarks/>
        public event RepositoryItemListCompletedEventHandler RepositoryItemListCompleted;

        /// <remarks/>
        public event RepositoryItemGetCompletedEventHandler RepositoryItemGetCompleted;

        /// <remarks/>
        public event RepositoryItemPublishCompletedEventHandler RepositoryItemPublishCompleted;

        /// <remarks/>
        public event AcknowledgeNotificationCompletedEventHandler AcknowledgeNotificationCompleted;

        /// <remarks/>
        public event DeviceAssetInfoExportCompletedEventHandler DeviceAssetInfoExportCompleted;

        /// <remarks/>
        public event DeviceAssetInfoExport2CompletedEventHandler DeviceAssetInfoExport2Completed;

        /// <remarks/>
        public event ActiveIssuesListCompletedEventHandler ActiveIssuesListCompleted;

        /// <remarks/>
        public event PSACredentialsValidateCompletedEventHandler PSACredentialsValidateCompleted;

        /// <remarks/>
        public event CustomerDeviceReportCompletedEventHandler CustomerDeviceReportCompleted;

        /// <remarks/>
        public event CustomerDeviceReportGetCompletedEventHandler CustomerDeviceReportGetCompleted;

        /// <remarks/>
        public event CustomerListCompletedEventHandler CustomerListCompleted;

        /// <remarks/>
        public event CustomerListChildrenCompletedEventHandler CustomerListChildrenCompleted;

        /// <remarks/>
        public event DeviceGetCompletedEventHandler DeviceGetCompleted;

        /// <remarks/>
        public event DeviceGetStatusCompletedEventHandler DeviceGetStatusCompleted;

        /// <remarks/>
        public event DeviceListCompletedEventHandler DeviceListCompleted;

        /// <remarks/>
        public event SOAddCompletedEventHandler SOAddCompleted;

        /// <remarks/>
        public event CustomerAddCompletedEventHandler CustomerAddCompleted;

        /// <remarks/>
        public event UserAddCompletedEventHandler UserAddCompleted;

        /// <remarks/>
        public event JobStatusListCompletedEventHandler JobStatusListCompleted;

        /// <remarks/>
        public event LastExportResetCompletedEventHandler LastExportResetCompleted;

        /// <remarks/>
        public event LastExportHardResetCompletedEventHandler LastExportHardResetCompleted;

        /// <remarks/>
        public event SystemSetAuditorSettingsCompletedEventHandler SystemSetAuditorSettingsCompleted;

        /// <remarks/>
        public event SystemSetAuditorSettings3CompletedEventHandler SystemSetAuditorSettings3Completed;

        /// <remarks/>
        public event SystemGetAuditorSettingsCompletedEventHandler SystemGetAuditorSettingsCompleted;

        /// <remarks/>
        public event SystemVerifyAuditorSettingsCompletedEventHandler SystemVerifyAuditorSettingsCompleted;

        /// <remarks/>
        public event SystemScheduleDataPushCompletedEventHandler SystemScheduleDataPushCompleted;

        /// <remarks/>
        public event SystemGetUserCompletedEventHandler SystemGetUserCompleted;

        /// <remarks/>
        public event SystemSetMonitoringTasksCompletedEventHandler SystemSetMonitoringTasksCompleted;

        /// <remarks/>
        public event SystemGetSessionIDCompletedEventHandler SystemGetSessionIDCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("RepositoryItemArray")]
        public RepositoryItem[] RepositoryItemList(string Username, string Password, int CustomerID, bool IncludeDefaultItems)
        {
            object[] results = this.Invoke("RepositoryItemList", new object[] {
                        Username,
                        Password,
                        CustomerID,
                        IncludeDefaultItems});
            return ((RepositoryItem[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginRepositoryItemList(string Username, string Password, int CustomerID, bool IncludeDefaultItems, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RepositoryItemList", new object[] {
                        Username,
                        Password,
                        CustomerID,
                        IncludeDefaultItems}, callback, asyncState);
        }

        /// <remarks/>
        public RepositoryItem[] EndRepositoryItemList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((RepositoryItem[])(results[0]));
        }

        /// <remarks/>
        public void RepositoryItemListAsync(string Username, string Password, int CustomerID, bool IncludeDefaultItems)
        {
            this.RepositoryItemListAsync(Username, Password, CustomerID, IncludeDefaultItems, null);
        }

        /// <remarks/>
        public void RepositoryItemListAsync(string Username, string Password, int CustomerID, bool IncludeDefaultItems, object userState)
        {
            if ((this.RepositoryItemListOperationCompleted == null))
            {
                this.RepositoryItemListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRepositoryItemListOperationCompleted);
            }
            this.InvokeAsync("RepositoryItemList", new object[] {
                        Username,
                        Password,
                        CustomerID,
                        IncludeDefaultItems}, this.RepositoryItemListOperationCompleted, userState);
        }

        private void OnRepositoryItemListOperationCompleted(object arg)
        {
            if ((this.RepositoryItemListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RepositoryItemListCompleted(this, new RepositoryItemListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("RepositoryItem")]
        public RepositoryItem RepositoryItemGet(string Username, string Password, string RepositoryItemUUID)
        {
            object[] results = this.Invoke("RepositoryItemGet", new object[] {
                        Username,
                        Password,
                        RepositoryItemUUID});
            return ((RepositoryItem)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginRepositoryItemGet(string Username, string Password, string RepositoryItemUUID, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RepositoryItemGet", new object[] {
                        Username,
                        Password,
                        RepositoryItemUUID}, callback, asyncState);
        }

        /// <remarks/>
        public RepositoryItem EndRepositoryItemGet(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((RepositoryItem)(results[0]));
        }

        /// <remarks/>
        public void RepositoryItemGetAsync(string Username, string Password, string RepositoryItemUUID)
        {
            this.RepositoryItemGetAsync(Username, Password, RepositoryItemUUID, null);
        }

        /// <remarks/>
        public void RepositoryItemGetAsync(string Username, string Password, string RepositoryItemUUID, object userState)
        {
            if ((this.RepositoryItemGetOperationCompleted == null))
            {
                this.RepositoryItemGetOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRepositoryItemGetOperationCompleted);
            }
            this.InvokeAsync("RepositoryItemGet", new object[] {
                        Username,
                        Password,
                        RepositoryItemUUID}, this.RepositoryItemGetOperationCompleted, userState);
        }

        private void OnRepositoryItemGetOperationCompleted(object arg)
        {
            if ((this.RepositoryItemGetCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RepositoryItemGetCompleted(this, new RepositoryItemGetCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("LastUploadDate")]
        public System.DateTime RepositoryItemPublish(string Username, string Password, int CustomerID, RepositoryItem RepositoryItem, bool Overwrite)
        {
            object[] results = this.Invoke("RepositoryItemPublish", new object[] {
                        Username,
                        Password,
                        CustomerID,
                        RepositoryItem,
                        Overwrite});
            return ((System.DateTime)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginRepositoryItemPublish(string Username, string Password, int CustomerID, RepositoryItem RepositoryItem, bool Overwrite, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("RepositoryItemPublish", new object[] {
                        Username,
                        Password,
                        CustomerID,
                        RepositoryItem,
                        Overwrite}, callback, asyncState);
        }

        /// <remarks/>
        public System.DateTime EndRepositoryItemPublish(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((System.DateTime)(results[0]));
        }

        /// <remarks/>
        public void RepositoryItemPublishAsync(string Username, string Password, int CustomerID, RepositoryItem RepositoryItem, bool Overwrite)
        {
            this.RepositoryItemPublishAsync(Username, Password, CustomerID, RepositoryItem, Overwrite, null);
        }

        /// <remarks/>
        public void RepositoryItemPublishAsync(string Username, string Password, int CustomerID, RepositoryItem RepositoryItem, bool Overwrite, object userState)
        {
            if ((this.RepositoryItemPublishOperationCompleted == null))
            {
                this.RepositoryItemPublishOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRepositoryItemPublishOperationCompleted);
            }
            this.InvokeAsync("RepositoryItemPublish", new object[] {
                        Username,
                        Password,
                        CustomerID,
                        RepositoryItem,
                        Overwrite}, this.RepositoryItemPublishOperationCompleted, userState);
        }

        private void OnRepositoryItemPublishOperationCompleted(object arg)
        {
            if ((this.RepositoryItemPublishCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RepositoryItemPublishCompleted(this, new RepositoryItemPublishCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        public void AcknowledgeNotification(int[] ActiveNotificationTriggerIDArray, int[] CorrelatedProfileIDArray, string Username, string Password, string Note, bool AddToDeviceNotes, bool SuppressOnEscalation)
        {
            this.Invoke("AcknowledgeNotification", new object[] {
                        ActiveNotificationTriggerIDArray,
                        CorrelatedProfileIDArray,
                        Username,
                        Password,
                        Note,
                        AddToDeviceNotes,
                        SuppressOnEscalation});
        }

        /// <remarks/>
        public System.IAsyncResult BeginAcknowledgeNotification(int[] ActiveNotificationTriggerIDArray, int[] CorrelatedProfileIDArray, string Username, string Password, string Note, bool AddToDeviceNotes, bool SuppressOnEscalation, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("AcknowledgeNotification", new object[] {
                        ActiveNotificationTriggerIDArray,
                        CorrelatedProfileIDArray,
                        Username,
                        Password,
                        Note,
                        AddToDeviceNotes,
                        SuppressOnEscalation}, callback, asyncState);
        }

        /// <remarks/>
        public void EndAcknowledgeNotification(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void AcknowledgeNotificationAsync(int[] ActiveNotificationTriggerIDArray, int[] CorrelatedProfileIDArray, string Username, string Password, string Note, bool AddToDeviceNotes, bool SuppressOnEscalation)
        {
            this.AcknowledgeNotificationAsync(ActiveNotificationTriggerIDArray, CorrelatedProfileIDArray, Username, Password, Note, AddToDeviceNotes, SuppressOnEscalation, null);
        }

        /// <remarks/>
        public void AcknowledgeNotificationAsync(int[] ActiveNotificationTriggerIDArray, int[] CorrelatedProfileIDArray, string Username, string Password, string Note, bool AddToDeviceNotes, bool SuppressOnEscalation, object userState)
        {
            if ((this.AcknowledgeNotificationOperationCompleted == null))
            {
                this.AcknowledgeNotificationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAcknowledgeNotificationOperationCompleted);
            }
            this.InvokeAsync("AcknowledgeNotification", new object[] {
                        ActiveNotificationTriggerIDArray,
                        CorrelatedProfileIDArray,
                        Username,
                        Password,
                        Note,
                        AddToDeviceNotes,
                        SuppressOnEscalation}, this.AcknowledgeNotificationOperationCompleted, userState);
        }

        private void OnAcknowledgeNotificationOperationCompleted(object arg)
        {
            if ((this.AcknowledgeNotificationCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AcknowledgeNotificationCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Data")]
        public DeviceData[] DeviceAssetInfoExport(string Version, string Username, string Password)
        {
            object[] results = this.Invoke("DeviceAssetInfoExport", new object[] {
                        Version,
                        Username,
                        Password});
            return ((DeviceData[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginDeviceAssetInfoExport(string Version, string Username, string Password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeviceAssetInfoExport", new object[] {
                        Version,
                        Username,
                        Password}, callback, asyncState);
        }

        /// <remarks/>
        public DeviceData[] EndDeviceAssetInfoExport(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeviceData[])(results[0]));
        }

        /// <remarks/>
        public void DeviceAssetInfoExportAsync(string Version, string Username, string Password)
        {
            this.DeviceAssetInfoExportAsync(Version, Username, Password, null);
        }

        /// <remarks/>
        public void DeviceAssetInfoExportAsync(string Version, string Username, string Password, object userState)
        {
            if ((this.DeviceAssetInfoExportOperationCompleted == null))
            {
                this.DeviceAssetInfoExportOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeviceAssetInfoExportOperationCompleted);
            }
            this.InvokeAsync("DeviceAssetInfoExport", new object[] {
                        Version,
                        Username,
                        Password}, this.DeviceAssetInfoExportOperationCompleted, userState);
        }

        private void OnDeviceAssetInfoExportOperationCompleted(object arg)
        {
            if ((this.DeviceAssetInfoExportCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeviceAssetInfoExportCompleted(this, new DeviceAssetInfoExportCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Data")]
        public Device[] DeviceAssetInfoExport2(string Version, string Username, string Password)
        {
            object[] results = this.Invoke("DeviceAssetInfoExport2", new object[] {
                        Version,
                        Username,
                        Password});
            return ((Device[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginDeviceAssetInfoExport2(string Version, string Username, string Password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeviceAssetInfoExport2", new object[] {
                        Version,
                        Username,
                        Password}, callback, asyncState);
        }

        /// <remarks/>
        public Device[] EndDeviceAssetInfoExport2(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((Device[])(results[0]));
        }

        /// <remarks/>
        public void DeviceAssetInfoExport2Async(string Version, string Username, string Password)
        {
            this.DeviceAssetInfoExport2Async(Version, Username, Password, null);
        }

        /// <remarks/>
        public void DeviceAssetInfoExport2Async(string Version, string Username, string Password, object userState)
        {
            if ((this.DeviceAssetInfoExport2OperationCompleted == null))
            {
                this.DeviceAssetInfoExport2OperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeviceAssetInfoExport2OperationCompleted);
            }
            this.InvokeAsync("DeviceAssetInfoExport2", new object[] {
                        Version,
                        Username,
                        Password}, this.DeviceAssetInfoExport2OperationCompleted, userState);
        }

        private void OnDeviceAssetInfoExport2OperationCompleted(object arg)
        {
            if ((this.DeviceAssetInfoExport2Completed != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeviceAssetInfoExport2Completed(this, new DeviceAssetInfoExport2CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Data")]
        public DeviceIssue[] ActiveIssuesList(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("ActiveIssuesList", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((DeviceIssue[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginActiveIssuesList(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("ActiveIssuesList", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public DeviceIssue[] EndActiveIssuesList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeviceIssue[])(results[0]));
        }

        /// <remarks/>
        public void ActiveIssuesListAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.ActiveIssuesListAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void ActiveIssuesListAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.ActiveIssuesListOperationCompleted == null))
            {
                this.ActiveIssuesListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnActiveIssuesListOperationCompleted);
            }
            this.InvokeAsync("ActiveIssuesList", new object[] {
                        Username,
                        Password,
                        Settings}, this.ActiveIssuesListOperationCompleted, userState);
        }

        private void OnActiveIssuesListOperationCompleted(object arg)
        {
            if ((this.ActiveIssuesListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ActiveIssuesListCompleted(this, new ActiveIssuesListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Data")]
        public bool PSACredentialsValidate(string Version, string Username, string Password)
        {
            object[] results = this.Invoke("PSACredentialsValidate", new object[] {
                        Version,
                        Username,
                        Password});
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginPSACredentialsValidate(string Version, string Username, string Password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("PSACredentialsValidate", new object[] {
                        Version,
                        Username,
                        Password}, callback, asyncState);
        }

        /// <remarks/>
        public bool EndPSACredentialsValidate(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public void PSACredentialsValidateAsync(string Version, string Username, string Password)
        {
            this.PSACredentialsValidateAsync(Version, Username, Password, null);
        }

        /// <remarks/>
        public void PSACredentialsValidateAsync(string Version, string Username, string Password, object userState)
        {
            if ((this.PSACredentialsValidateOperationCompleted == null))
            {
                this.PSACredentialsValidateOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPSACredentialsValidateOperationCompleted);
            }
            this.InvokeAsync("PSACredentialsValidate", new object[] {
                        Version,
                        Username,
                        Password}, this.PSACredentialsValidateOperationCompleted, userState);
        }

        private void OnPSACredentialsValidateOperationCompleted(object arg)
        {
            if ((this.PSACredentialsValidateCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PSACredentialsValidateCompleted(this, new PSACredentialsValidateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("ReportData")]
        public DeviceReportData[] CustomerDeviceReport(string Version, string Username, string Password, System.DateTime StartDate, System.DateTime EndDate)
        {
            object[] results = this.Invoke("CustomerDeviceReport", new object[] {
                        Version,
                        Username,
                        Password,
                        StartDate,
                        EndDate});
            return ((DeviceReportData[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginCustomerDeviceReport(string Version, string Username, string Password, System.DateTime StartDate, System.DateTime EndDate, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CustomerDeviceReport", new object[] {
                        Version,
                        Username,
                        Password,
                        StartDate,
                        EndDate}, callback, asyncState);
        }

        /// <remarks/>
        public DeviceReportData[] EndCustomerDeviceReport(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeviceReportData[])(results[0]));
        }

        /// <remarks/>
        public void CustomerDeviceReportAsync(string Version, string Username, string Password, System.DateTime StartDate, System.DateTime EndDate)
        {
            this.CustomerDeviceReportAsync(Version, Username, Password, StartDate, EndDate, null);
        }

        /// <remarks/>
        public void CustomerDeviceReportAsync(string Version, string Username, string Password, System.DateTime StartDate, System.DateTime EndDate, object userState)
        {
            if ((this.CustomerDeviceReportOperationCompleted == null))
            {
                this.CustomerDeviceReportOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCustomerDeviceReportOperationCompleted);
            }
            this.InvokeAsync("CustomerDeviceReport", new object[] {
                        Version,
                        Username,
                        Password,
                        StartDate,
                        EndDate}, this.CustomerDeviceReportOperationCompleted, userState);
        }

        private void OnCustomerDeviceReportOperationCompleted(object arg)
        {
            if ((this.CustomerDeviceReportCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CustomerDeviceReportCompleted(this, new CustomerDeviceReportCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("ReportData")]
        public DeviceReportData[] CustomerDeviceReportGet(string Version, string Username, string Password)
        {
            object[] results = this.Invoke("CustomerDeviceReportGet", new object[] {
                        Version,
                        Username,
                        Password});
            return ((DeviceReportData[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginCustomerDeviceReportGet(string Version, string Username, string Password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CustomerDeviceReportGet", new object[] {
                        Version,
                        Username,
                        Password}, callback, asyncState);
        }

        /// <remarks/>
        public DeviceReportData[] EndCustomerDeviceReportGet(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeviceReportData[])(results[0]));
        }

        /// <remarks/>
        public void CustomerDeviceReportGetAsync(string Version, string Username, string Password)
        {
            this.CustomerDeviceReportGetAsync(Version, Username, Password, null);
        }

        /// <remarks/>
        public void CustomerDeviceReportGetAsync(string Version, string Username, string Password, object userState)
        {
            if ((this.CustomerDeviceReportGetOperationCompleted == null))
            {
                this.CustomerDeviceReportGetOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCustomerDeviceReportGetOperationCompleted);
            }
            this.InvokeAsync("CustomerDeviceReportGet", new object[] {
                        Version,
                        Username,
                        Password}, this.CustomerDeviceReportGetOperationCompleted, userState);
        }

        private void OnCustomerDeviceReportGetOperationCompleted(object arg)
        {
            if ((this.CustomerDeviceReportGetCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CustomerDeviceReportGetCompleted(this, new CustomerDeviceReportGetCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Data")]
        public Customer[] CustomerList(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("CustomerList", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((Customer[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginCustomerList(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CustomerList", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public Customer[] EndCustomerList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((Customer[])(results[0]));
        }

        /// <remarks/>
        public void CustomerListAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.CustomerListAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void CustomerListAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.CustomerListOperationCompleted == null))
            {
                this.CustomerListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCustomerListOperationCompleted);
            }
            this.InvokeAsync("CustomerList", new object[] {
                        Username,
                        Password,
                        Settings}, this.CustomerListOperationCompleted, userState);
        }

        private void OnCustomerListOperationCompleted(object arg)
        {
            if ((this.CustomerListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CustomerListCompleted(this, new CustomerListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Data")]
        public Customer[] CustomerListChildren(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("CustomerListChildren", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((Customer[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginCustomerListChildren(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CustomerListChildren", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public Customer[] EndCustomerListChildren(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((Customer[])(results[0]));
        }

        /// <remarks/>
        public void CustomerListChildrenAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.CustomerListChildrenAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void CustomerListChildrenAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.CustomerListChildrenOperationCompleted == null))
            {
                this.CustomerListChildrenOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCustomerListChildrenOperationCompleted);
            }
            this.InvokeAsync("CustomerListChildren", new object[] {
                        Username,
                        Password,
                        Settings}, this.CustomerListChildrenOperationCompleted, userState);
        }

        private void OnCustomerListChildrenOperationCompleted(object arg)
        {
            if ((this.CustomerListChildrenCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CustomerListChildrenCompleted(this, new CustomerListChildrenCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Devices")]
        public DeviceInfo[] DeviceGet(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("DeviceGet", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((DeviceInfo[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginDeviceGet(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeviceGet", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public DeviceInfo[] EndDeviceGet(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeviceInfo[])(results[0]));
        }

        /// <remarks/>
        public void DeviceGetAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.DeviceGetAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void DeviceGetAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.DeviceGetOperationCompleted == null))
            {
                this.DeviceGetOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeviceGetOperationCompleted);
            }
            this.InvokeAsync("DeviceGet", new object[] {
                        Username,
                        Password,
                        Settings}, this.DeviceGetOperationCompleted, userState);
        }

        private void OnDeviceGetOperationCompleted(object arg)
        {
            if ((this.DeviceGetCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeviceGetCompleted(this, new DeviceGetCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("DeviceStatuses")]
        public DeviceStatus[] DeviceGetStatus(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("DeviceGetStatus", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((DeviceStatus[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginDeviceGetStatus(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeviceGetStatus", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public DeviceStatus[] EndDeviceGetStatus(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((DeviceStatus[])(results[0]));
        }

        /// <remarks/>
        public void DeviceGetStatusAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.DeviceGetStatusAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void DeviceGetStatusAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.DeviceGetStatusOperationCompleted == null))
            {
                this.DeviceGetStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeviceGetStatusOperationCompleted);
            }
            this.InvokeAsync("DeviceGetStatus", new object[] {
                        Username,
                        Password,
                        Settings}, this.DeviceGetStatusOperationCompleted, userState);
        }

        private void OnDeviceGetStatusOperationCompleted(object arg)
        {
            if ((this.DeviceGetStatusCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeviceGetStatusCompleted(this, new DeviceGetStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Devices")]
        public Device[] DeviceList(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("DeviceList", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((Device[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginDeviceList(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("DeviceList", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public Device[] EndDeviceList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((Device[])(results[0]));
        }

        /// <remarks/>
        public void DeviceListAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.DeviceListAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void DeviceListAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.DeviceListOperationCompleted == null))
            {
                this.DeviceListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeviceListOperationCompleted);
            }
            this.InvokeAsync("DeviceList", new object[] {
                        Username,
                        Password,
                        Settings}, this.DeviceListOperationCompleted, userState);
        }

        private void OnDeviceListOperationCompleted(object arg)
        {
            if ((this.DeviceListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeviceListCompleted(this, new DeviceListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("SOID")]
        public int SOAdd(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("SOAdd", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((int)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginSOAdd(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SOAdd", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public int EndSOAdd(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((int)(results[0]));
        }

        /// <remarks/>
        public void SOAddAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.SOAddAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void SOAddAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.SOAddOperationCompleted == null))
            {
                this.SOAddOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSOAddOperationCompleted);
            }
            this.InvokeAsync("SOAdd", new object[] {
                        Username,
                        Password,
                        Settings}, this.SOAddOperationCompleted, userState);
        }

        private void OnSOAddOperationCompleted(object arg)
        {
            if ((this.SOAddCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SOAddCompleted(this, new SOAddCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("CustomerID")]
        public int CustomerAdd(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("CustomerAdd", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((int)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginCustomerAdd(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("CustomerAdd", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public int EndCustomerAdd(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((int)(results[0]));
        }

        /// <remarks/>
        public void CustomerAddAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.CustomerAddAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void CustomerAddAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.CustomerAddOperationCompleted == null))
            {
                this.CustomerAddOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCustomerAddOperationCompleted);
            }
            this.InvokeAsync("CustomerAdd", new object[] {
                        Username,
                        Password,
                        Settings}, this.CustomerAddOperationCompleted, userState);
        }

        private void OnCustomerAddOperationCompleted(object arg)
        {
            if ((this.CustomerAddCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CustomerAddCompleted(this, new CustomerAddCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("UserID")]
        public int UserAdd(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("UserAdd", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((int)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginUserAdd(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("UserAdd", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public int EndUserAdd(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((int)(results[0]));
        }

        /// <remarks/>
        public void UserAddAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.UserAddAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void UserAddAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.UserAddOperationCompleted == null))
            {
                this.UserAddOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUserAddOperationCompleted);
            }
            this.InvokeAsync("UserAdd", new object[] {
                        Username,
                        Password,
                        Settings}, this.UserAddOperationCompleted, userState);
        }

        private void OnUserAddOperationCompleted(object arg)
        {
            if ((this.UserAddCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UserAddCompleted(this, new UserAddCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("JobStatuses")]
        public JobStatus[] JobStatusList(string Username, string Password, T_KeyPair[] Settings)
        {
            object[] results = this.Invoke("JobStatusList", new object[] {
                        Username,
                        Password,
                        Settings});
            return ((JobStatus[])(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginJobStatusList(string Username, string Password, T_KeyPair[] Settings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("JobStatusList", new object[] {
                        Username,
                        Password,
                        Settings}, callback, asyncState);
        }

        /// <remarks/>
        public JobStatus[] EndJobStatusList(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((JobStatus[])(results[0]));
        }

        /// <remarks/>
        public void JobStatusListAsync(string Username, string Password, T_KeyPair[] Settings)
        {
            this.JobStatusListAsync(Username, Password, Settings, null);
        }

        /// <remarks/>
        public void JobStatusListAsync(string Username, string Password, T_KeyPair[] Settings, object userState)
        {
            if ((this.JobStatusListOperationCompleted == null))
            {
                this.JobStatusListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnJobStatusListOperationCompleted);
            }
            this.InvokeAsync("JobStatusList", new object[] {
                        Username,
                        Password,
                        Settings}, this.JobStatusListOperationCompleted, userState);
        }

        private void OnJobStatusListOperationCompleted(object arg)
        {
            if ((this.JobStatusListCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.JobStatusListCompleted(this, new JobStatusListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        public void LastExportReset(string Version, string Username, string Password)
        {
            this.Invoke("LastExportReset", new object[] {
                        Version,
                        Username,
                        Password});
        }

        /// <remarks/>
        public System.IAsyncResult BeginLastExportReset(string Version, string Username, string Password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("LastExportReset", new object[] {
                        Version,
                        Username,
                        Password}, callback, asyncState);
        }

        /// <remarks/>
        public void EndLastExportReset(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void LastExportResetAsync(string Version, string Username, string Password)
        {
            this.LastExportResetAsync(Version, Username, Password, null);
        }

        /// <remarks/>
        public void LastExportResetAsync(string Version, string Username, string Password, object userState)
        {
            if ((this.LastExportResetOperationCompleted == null))
            {
                this.LastExportResetOperationCompleted = new System.Threading.SendOrPostCallback(this.OnLastExportResetOperationCompleted);
            }
            this.InvokeAsync("LastExportReset", new object[] {
                        Version,
                        Username,
                        Password}, this.LastExportResetOperationCompleted, userState);
        }

        private void OnLastExportResetOperationCompleted(object arg)
        {
            if ((this.LastExportResetCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.LastExportResetCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        public void LastExportHardReset(string Version, string Username, string Password)
        {
            this.Invoke("LastExportHardReset", new object[] {
                        Version,
                        Username,
                        Password});
        }

        /// <remarks/>
        public System.IAsyncResult BeginLastExportHardReset(string Version, string Username, string Password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("LastExportHardReset", new object[] {
                        Version,
                        Username,
                        Password}, callback, asyncState);
        }

        /// <remarks/>
        public void EndLastExportHardReset(System.IAsyncResult asyncResult)
        {
            this.EndInvoke(asyncResult);
        }

        /// <remarks/>
        public void LastExportHardResetAsync(string Version, string Username, string Password)
        {
            this.LastExportHardResetAsync(Version, Username, Password, null);
        }

        /// <remarks/>
        public void LastExportHardResetAsync(string Version, string Username, string Password, object userState)
        {
            if ((this.LastExportHardResetOperationCompleted == null))
            {
                this.LastExportHardResetOperationCompleted = new System.Threading.SendOrPostCallback(this.OnLastExportHardResetOperationCompleted);
            }
            this.InvokeAsync("LastExportHardReset", new object[] {
                        Version,
                        Username,
                        Password}, this.LastExportHardResetOperationCompleted, userState);
        }

        private void OnLastExportHardResetOperationCompleted(object arg)
        {
            if ((this.LastExportHardResetCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.LastExportHardResetCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Msg")]
        public string SystemSetAuditorSettings(T_Credentials Credentials, bool AuditorEnabled, T_Pair[] AuditorSettings, out T_Credentials NewCredentials)
        {
            object[] results = this.Invoke("SystemSetAuditorSettings", new object[] {
                        Credentials,
                        AuditorEnabled,
                        AuditorSettings});
            NewCredentials = ((T_Credentials)(results[1]));
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginSystemSetAuditorSettings(T_Credentials Credentials, bool AuditorEnabled, T_Pair[] AuditorSettings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SystemSetAuditorSettings", new object[] {
                        Credentials,
                        AuditorEnabled,
                        AuditorSettings}, callback, asyncState);
        }

        /// <remarks/>
        public string EndSystemSetAuditorSettings(System.IAsyncResult asyncResult, out T_Credentials NewCredentials)
        {
            object[] results = this.EndInvoke(asyncResult);
            NewCredentials = ((T_Credentials)(results[1]));
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void SystemSetAuditorSettingsAsync(T_Credentials Credentials, bool AuditorEnabled, T_Pair[] AuditorSettings)
        {
            this.SystemSetAuditorSettingsAsync(Credentials, AuditorEnabled, AuditorSettings, null);
        }

        /// <remarks/>
        public void SystemSetAuditorSettingsAsync(T_Credentials Credentials, bool AuditorEnabled, T_Pair[] AuditorSettings, object userState)
        {
            if ((this.SystemSetAuditorSettingsOperationCompleted == null))
            {
                this.SystemSetAuditorSettingsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSystemSetAuditorSettingsOperationCompleted);
            }
            this.InvokeAsync("SystemSetAuditorSettings", new object[] {
                        Credentials,
                        AuditorEnabled,
                        AuditorSettings}, this.SystemSetAuditorSettingsOperationCompleted, userState);
        }

        private void OnSystemSetAuditorSettingsOperationCompleted(object arg)
        {
            if ((this.SystemSetAuditorSettingsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SystemSetAuditorSettingsCompleted(this, new SystemSetAuditorSettingsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Msg")]
        public string SystemSetAuditorSettings3(T_Credentials Credentials, bool AuditorEnabled, T_Pair[] AuditorSettings, out string SelfMonitoringACEP, out T_Credentials NewCredentials)
        {
            object[] results = this.Invoke("SystemSetAuditorSettings3", new object[] {
                        Credentials,
                        AuditorEnabled,
                        AuditorSettings});
            SelfMonitoringACEP = ((string)(results[1]));
            NewCredentials = ((T_Credentials)(results[2]));
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginSystemSetAuditorSettings3(T_Credentials Credentials, bool AuditorEnabled, T_Pair[] AuditorSettings, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SystemSetAuditorSettings3", new object[] {
                        Credentials,
                        AuditorEnabled,
                        AuditorSettings}, callback, asyncState);
        }

        /// <remarks/>
        public string EndSystemSetAuditorSettings3(System.IAsyncResult asyncResult, out string SelfMonitoringACEP, out T_Credentials NewCredentials)
        {
            object[] results = this.EndInvoke(asyncResult);
            SelfMonitoringACEP = ((string)(results[1]));
            NewCredentials = ((T_Credentials)(results[2]));
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void SystemSetAuditorSettings3Async(T_Credentials Credentials, bool AuditorEnabled, T_Pair[] AuditorSettings)
        {
            this.SystemSetAuditorSettings3Async(Credentials, AuditorEnabled, AuditorSettings, null);
        }

        /// <remarks/>
        public void SystemSetAuditorSettings3Async(T_Credentials Credentials, bool AuditorEnabled, T_Pair[] AuditorSettings, object userState)
        {
            if ((this.SystemSetAuditorSettings3OperationCompleted == null))
            {
                this.SystemSetAuditorSettings3OperationCompleted = new System.Threading.SendOrPostCallback(this.OnSystemSetAuditorSettings3OperationCompleted);
            }
            this.InvokeAsync("SystemSetAuditorSettings3", new object[] {
                        Credentials,
                        AuditorEnabled,
                        AuditorSettings}, this.SystemSetAuditorSettings3OperationCompleted, userState);
        }

        private void OnSystemSetAuditorSettings3OperationCompleted(object arg)
        {
            if ((this.SystemSetAuditorSettings3Completed != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SystemSetAuditorSettings3Completed(this, new SystemSetAuditorSettings3CompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("AuditorSettings")]
        public T_AuditorSettings SystemGetAuditorSettings(T_Credentials Credentials)
        {
            object[] results = this.Invoke("SystemGetAuditorSettings", new object[] {
                        Credentials});
            return ((T_AuditorSettings)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginSystemGetAuditorSettings(T_Credentials Credentials, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SystemGetAuditorSettings", new object[] {
                        Credentials}, callback, asyncState);
        }

        /// <remarks/>
        public T_AuditorSettings EndSystemGetAuditorSettings(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((T_AuditorSettings)(results[0]));
        }

        /// <remarks/>
        public void SystemGetAuditorSettingsAsync(T_Credentials Credentials)
        {
            this.SystemGetAuditorSettingsAsync(Credentials, null);
        }

        /// <remarks/>
        public void SystemGetAuditorSettingsAsync(T_Credentials Credentials, object userState)
        {
            if ((this.SystemGetAuditorSettingsOperationCompleted == null))
            {
                this.SystemGetAuditorSettingsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSystemGetAuditorSettingsOperationCompleted);
            }
            this.InvokeAsync("SystemGetAuditorSettings", new object[] {
                        Credentials}, this.SystemGetAuditorSettingsOperationCompleted, userState);
        }

        private void OnSystemGetAuditorSettingsOperationCompleted(object arg)
        {
            if ((this.SystemGetAuditorSettingsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SystemGetAuditorSettingsCompleted(this, new SystemGetAuditorSettingsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("ResultCode")]
        public int SystemVerifyAuditorSettings(T_Credentials Credentials, out string DetailedMessage)
        {
            object[] results = this.Invoke("SystemVerifyAuditorSettings", new object[] {
                        Credentials});
            DetailedMessage = ((string)(results[1]));
            return ((int)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginSystemVerifyAuditorSettings(T_Credentials Credentials, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SystemVerifyAuditorSettings", new object[] {
                        Credentials}, callback, asyncState);
        }

        /// <remarks/>
        public int EndSystemVerifyAuditorSettings(System.IAsyncResult asyncResult, out string DetailedMessage)
        {
            object[] results = this.EndInvoke(asyncResult);
            DetailedMessage = ((string)(results[1]));
            return ((int)(results[0]));
        }

        /// <remarks/>
        public void SystemVerifyAuditorSettingsAsync(T_Credentials Credentials)
        {
            this.SystemVerifyAuditorSettingsAsync(Credentials, null);
        }

        /// <remarks/>
        public void SystemVerifyAuditorSettingsAsync(T_Credentials Credentials, object userState)
        {
            if ((this.SystemVerifyAuditorSettingsOperationCompleted == null))
            {
                this.SystemVerifyAuditorSettingsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSystemVerifyAuditorSettingsOperationCompleted);
            }
            this.InvokeAsync("SystemVerifyAuditorSettings", new object[] {
                        Credentials}, this.SystemVerifyAuditorSettingsOperationCompleted, userState);
        }

        private void OnSystemVerifyAuditorSettingsOperationCompleted(object arg)
        {
            if ((this.SystemVerifyAuditorSettingsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SystemVerifyAuditorSettingsCompleted(this, new SystemVerifyAuditorSettingsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Msg")]
        public string SystemScheduleDataPush(T_Credentials Credentials)
        {
            object[] results = this.Invoke("SystemScheduleDataPush", new object[] {
                        Credentials});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginSystemScheduleDataPush(T_Credentials Credentials, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SystemScheduleDataPush", new object[] {
                        Credentials}, callback, asyncState);
        }

        /// <remarks/>
        public string EndSystemScheduleDataPush(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void SystemScheduleDataPushAsync(T_Credentials Credentials)
        {
            this.SystemScheduleDataPushAsync(Credentials, null);
        }

        /// <remarks/>
        public void SystemScheduleDataPushAsync(T_Credentials Credentials, object userState)
        {
            if ((this.SystemScheduleDataPushOperationCompleted == null))
            {
                this.SystemScheduleDataPushOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSystemScheduleDataPushOperationCompleted);
            }
            this.InvokeAsync("SystemScheduleDataPush", new object[] {
                        Credentials}, this.SystemScheduleDataPushOperationCompleted, userState);
        }

        private void OnSystemScheduleDataPushOperationCompleted(object arg)
        {
            if ((this.SystemScheduleDataPushCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SystemScheduleDataPushCompleted(this, new SystemScheduleDataPushCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("User")]
        public T_User SystemGetUser(T_Credentials Credentials, int SessionID)
        {
            object[] results = this.Invoke("SystemGetUser", new object[] {
                        Credentials,
                        SessionID});
            return ((T_User)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginSystemGetUser(T_Credentials Credentials, int SessionID, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SystemGetUser", new object[] {
                        Credentials,
                        SessionID}, callback, asyncState);
        }

        /// <remarks/>
        public T_User EndSystemGetUser(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((T_User)(results[0]));
        }

        /// <remarks/>
        public void SystemGetUserAsync(T_Credentials Credentials, int SessionID)
        {
            this.SystemGetUserAsync(Credentials, SessionID, null);
        }

        /// <remarks/>
        public void SystemGetUserAsync(T_Credentials Credentials, int SessionID, object userState)
        {
            if ((this.SystemGetUserOperationCompleted == null))
            {
                this.SystemGetUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSystemGetUserOperationCompleted);
            }
            this.InvokeAsync("SystemGetUser", new object[] {
                        Credentials,
                        SessionID}, this.SystemGetUserOperationCompleted, userState);
        }

        private void OnSystemGetUserOperationCompleted(object arg)
        {
            if ((this.SystemGetUserCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SystemGetUserCompleted(this, new SystemGetUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("Msg")]
        public string SystemSetMonitoringTasks(T_Credentials Credentials, T_Pair[] Parameters)
        {
            object[] results = this.Invoke("SystemSetMonitoringTasks", new object[] {
                        Credentials,
                        Parameters});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginSystemSetMonitoringTasks(T_Credentials Credentials, T_Pair[] Parameters, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SystemSetMonitoringTasks", new object[] {
                        Credentials,
                        Parameters}, callback, asyncState);
        }

        /// <remarks/>
        public string EndSystemSetMonitoringTasks(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void SystemSetMonitoringTasksAsync(T_Credentials Credentials, T_Pair[] Parameters)
        {
            this.SystemSetMonitoringTasksAsync(Credentials, Parameters, null);
        }

        /// <remarks/>
        public void SystemSetMonitoringTasksAsync(T_Credentials Credentials, T_Pair[] Parameters, object userState)
        {
            if ((this.SystemSetMonitoringTasksOperationCompleted == null))
            {
                this.SystemSetMonitoringTasksOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSystemSetMonitoringTasksOperationCompleted);
            }
            this.InvokeAsync("SystemSetMonitoringTasks", new object[] {
                        Credentials,
                        Parameters}, this.SystemSetMonitoringTasksOperationCompleted, userState);
        }

        private void OnSystemSetMonitoringTasksOperationCompleted(object arg)
        {
            if ((this.SystemSetMonitoringTasksCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SystemSetMonitoringTasksCompleted(this, new SystemSetMonitoringTasksCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace = "http://www.n-able.com/mickey", ResponseNamespace = "http://www.n-able.com/mickey")]
        [return: System.Xml.Serialization.SoapElementAttribute("SessionID")]
        public int SystemGetSessionID(T_Credentials Credentials, string Email, string Password)
        {
            object[] results = this.Invoke("SystemGetSessionID", new object[] {
                        Credentials,
                        Email,
                        Password});
            return ((int)(results[0]));
        }

        /// <remarks/>
        public System.IAsyncResult BeginSystemGetSessionID(T_Credentials Credentials, string Email, string Password, System.AsyncCallback callback, object asyncState)
        {
            return this.BeginInvoke("SystemGetSessionID", new object[] {
                        Credentials,
                        Email,
                        Password}, callback, asyncState);
        }

        /// <remarks/>
        public int EndSystemGetSessionID(System.IAsyncResult asyncResult)
        {
            object[] results = this.EndInvoke(asyncResult);
            return ((int)(results[0]));
        }

        /// <remarks/>
        public void SystemGetSessionIDAsync(T_Credentials Credentials, string Email, string Password)
        {
            this.SystemGetSessionIDAsync(Credentials, Email, Password, null);
        }

        /// <remarks/>
        public void SystemGetSessionIDAsync(T_Credentials Credentials, string Email, string Password, object userState)
        {
            if ((this.SystemGetSessionIDOperationCompleted == null))
            {
                this.SystemGetSessionIDOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSystemGetSessionIDOperationCompleted);
            }
            this.InvokeAsync("SystemGetSessionID", new object[] {
                        Credentials,
                        Email,
                        Password}, this.SystemGetSessionIDOperationCompleted, userState);
        }

        private void OnSystemGetSessionIDOperationCompleted(object arg)
        {
            if ((this.SystemGetSessionIDCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SystemGetSessionIDCompleted(this, new SystemGetSessionIDCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class RepositoryItem
    {

        private string repositoryItemUUIDField;

        private string nameField;

        private string descriptionField;

        private System.Nullable<System.DateTime> lastUploadDateField;

        private byte[] policyFileField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string RepositoryItemUUID
        {
            get
            {
                return this.repositoryItemUUIDField;
            }
            set
            {
                this.repositoryItemUUIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> LastUploadDate
        {
            get
            {
                return this.lastUploadDateField;
            }
            set
            {
                this.lastUploadDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(DataType = "base64Binary", IsNullable = true)]
        public byte[] PolicyFile
        {
            get
            {
                return this.policyFileField;
            }
            set
            {
                this.policyFileField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class T_User
    {

        private System.Nullable<int> userIDField;

        private System.Nullable<int> customerIDField;

        private string usernameField;

        private string passwordMD5Field;

        private string lastLoginField;

        private string titleField;

        private string firstNameField;

        private string lastNameField;

        private string email1Field;

        private string phone1Field;

        private string extension1Field;

        private string departmentField;

        private string street1Field;

        private string street2Field;

        private string cityField;

        private string stateProvField;

        private string countryField;

        private string postalCodeField;

        private System.Nullable<bool> isEnabledField;

        private string primaryAccountField;

        private System.Nullable<bool> autoSO_PowerField;

        private System.Nullable<bool> autoSO_UserField;

        private System.Nullable<bool> isLDAPField;

        private System.Nullable<bool> isLockedField;

        private System.Nullable<bool> isPasswordExpiredField;

        private System.Nullable<bool> ncompassEnabledField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> UserID
        {
            get
            {
                return this.userIDField;
            }
            set
            {
                this.userIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> CustomerID
        {
            get
            {
                return this.customerIDField;
            }
            set
            {
                this.customerIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Username
        {
            get
            {
                return this.usernameField;
            }
            set
            {
                this.usernameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string PasswordMD5
        {
            get
            {
                return this.passwordMD5Field;
            }
            set
            {
                this.passwordMD5Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string LastLogin
        {
            get
            {
                return this.lastLoginField;
            }
            set
            {
                this.lastLoginField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string FirstName
        {
            get
            {
                return this.firstNameField;
            }
            set
            {
                this.firstNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string LastName
        {
            get
            {
                return this.lastNameField;
            }
            set
            {
                this.lastNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Email1
        {
            get
            {
                return this.email1Field;
            }
            set
            {
                this.email1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Phone1
        {
            get
            {
                return this.phone1Field;
            }
            set
            {
                this.phone1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Extension1
        {
            get
            {
                return this.extension1Field;
            }
            set
            {
                this.extension1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Department
        {
            get
            {
                return this.departmentField;
            }
            set
            {
                this.departmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Street1
        {
            get
            {
                return this.street1Field;
            }
            set
            {
                this.street1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Street2
        {
            get
            {
                return this.street2Field;
            }
            set
            {
                this.street2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string StateProv
        {
            get
            {
                return this.stateProvField;
            }
            set
            {
                this.stateProvField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<bool> IsEnabled
        {
            get
            {
                return this.isEnabledField;
            }
            set
            {
                this.isEnabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string PrimaryAccount
        {
            get
            {
                return this.primaryAccountField;
            }
            set
            {
                this.primaryAccountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<bool> AutoSO_Power
        {
            get
            {
                return this.autoSO_PowerField;
            }
            set
            {
                this.autoSO_PowerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<bool> AutoSO_User
        {
            get
            {
                return this.autoSO_UserField;
            }
            set
            {
                this.autoSO_UserField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<bool> IsLDAP
        {
            get
            {
                return this.isLDAPField;
            }
            set
            {
                this.isLDAPField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<bool> IsLocked
        {
            get
            {
                return this.isLockedField;
            }
            set
            {
                this.isLockedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<bool> IsPasswordExpired
        {
            get
            {
                return this.isPasswordExpiredField;
            }
            set
            {
                this.isPasswordExpiredField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<bool> NcompassEnabled
        {
            get
            {
                return this.ncompassEnabledField;
            }
            set
            {
                this.ncompassEnabledField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class T_AuditorSettings
    {

        private string menuURLField;

        private string adminURLField;

        private string ncWSURLField;

        private int dataSourceIDField;

        private string exportIPField;

        private int exportPortField;

        private string exportODSField;

        private string exportUserField;

        private string exportPasswordField;

        private string ncompassExportVersionField;

        private string ncentralIPField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string MenuURL
        {
            get
            {
                return this.menuURLField;
            }
            set
            {
                this.menuURLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string AdminURL
        {
            get
            {
                return this.adminURLField;
            }
            set
            {
                this.adminURLField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string NcWSURL
        {
            get
            {
                return this.ncWSURLField;
            }
            set
            {
                this.ncWSURLField = value;
            }
        }

        /// <remarks/>
        public int DataSourceID
        {
            get
            {
                return this.dataSourceIDField;
            }
            set
            {
                this.dataSourceIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string ExportIP
        {
            get
            {
                return this.exportIPField;
            }
            set
            {
                this.exportIPField = value;
            }
        }

        /// <remarks/>
        public int ExportPort
        {
            get
            {
                return this.exportPortField;
            }
            set
            {
                this.exportPortField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string ExportODS
        {
            get
            {
                return this.exportODSField;
            }
            set
            {
                this.exportODSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string ExportUser
        {
            get
            {
                return this.exportUserField;
            }
            set
            {
                this.exportUserField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string ExportPassword
        {
            get
            {
                return this.exportPasswordField;
            }
            set
            {
                this.exportPasswordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string NcompassExportVersion
        {
            get
            {
                return this.ncompassExportVersionField;
            }
            set
            {
                this.ncompassExportVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string NcentralIP
        {
            get
            {
                return this.ncentralIPField;
            }
            set
            {
                this.ncentralIPField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class T_Pair
    {

        private string keyField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class T_Credentials
    {

        private string usernameField;

        private string passwordField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Username
        {
            get
            {
                return this.usernameField;
            }
            set
            {
                this.usernameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class JobStatus
    {

        private T_KeyPair[] infoField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public T_KeyPair[] Info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class T_KeyPair
    {

        private string keyField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class DeviceStatus
    {

        private T_KeyPair[] infoField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public T_KeyPair[] Info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class T_KeyValue
    {

        private string keyField;

        private string[] valueField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string Key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string[] Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class DeviceInfo
    {

        private T_KeyValue[] infoField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public T_KeyValue[] Info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class Customer
    {

        private T_KeyPair[] infoField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public T_KeyPair[] Info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class DeviceReportDeviceData
    {

        private System.Nullable<int> deviceIDField;

        private string deviceClassField;

        private System.Nullable<float> serverAvailabilityPercentageField;

        private DeviceHardDriveData[] hardDrivesField;

        private System.Nullable<float> cPUUtilizationPercentageField;

        private System.Nullable<float> memoryUtilizationPercentageField;

        private System.Nullable<int> numVirusesField;

        private System.Nullable<int> numBackupPassesField;

        private System.Nullable<int> numBackupWarningsField;

        private System.Nullable<int> numBackupFailuresField;

        private System.Nullable<int> numPatchAlertsField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> DeviceID
        {
            get
            {
                return this.deviceIDField;
            }
            set
            {
                this.deviceIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string DeviceClass
        {
            get
            {
                return this.deviceClassField;
            }
            set
            {
                this.deviceClassField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<float> ServerAvailabilityPercentage
        {
            get
            {
                return this.serverAvailabilityPercentageField;
            }
            set
            {
                this.serverAvailabilityPercentageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public DeviceHardDriveData[] HardDrives
        {
            get
            {
                return this.hardDrivesField;
            }
            set
            {
                this.hardDrivesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<float> CPUUtilizationPercentage
        {
            get
            {
                return this.cPUUtilizationPercentageField;
            }
            set
            {
                this.cPUUtilizationPercentageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<float> MemoryUtilizationPercentage
        {
            get
            {
                return this.memoryUtilizationPercentageField;
            }
            set
            {
                this.memoryUtilizationPercentageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumViruses
        {
            get
            {
                return this.numVirusesField;
            }
            set
            {
                this.numVirusesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumBackupPasses
        {
            get
            {
                return this.numBackupPassesField;
            }
            set
            {
                this.numBackupPassesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumBackupWarnings
        {
            get
            {
                return this.numBackupWarningsField;
            }
            set
            {
                this.numBackupWarningsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumBackupFailures
        {
            get
            {
                return this.numBackupFailuresField;
            }
            set
            {
                this.numBackupFailuresField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumPatchAlerts
        {
            get
            {
                return this.numPatchAlertsField;
            }
            set
            {
                this.numPatchAlertsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class DeviceHardDriveData
    {

        private System.Nullable<long> capacityBytesField;

        private System.Nullable<long> usedBytesField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<long> CapacityBytes
        {
            get
            {
                return this.capacityBytesField;
            }
            set
            {
                this.capacityBytesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<long> UsedBytes
        {
            get
            {
                return this.usedBytesField;
            }
            set
            {
                this.usedBytesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class DeviceReportData
    {

        private string customerNameField;

        private System.Nullable<int> customerIDField;

        private string sOCustomerNameField;

        private System.Nullable<int> sOCustomerIDField;

        private System.Nullable<int> numDevicesField;

        private System.Nullable<int> numServerDevicesField;

        private System.Nullable<int> numWorkstationDevicesField;

        private System.Nullable<int> numWinServerDevicesField;

        private System.Nullable<int> numWinWorkstationDevicesField;

        private System.Nullable<int> numLowDiskServersField;

        private DeviceReportDeviceData[] devicesField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string CustomerName
        {
            get
            {
                return this.customerNameField;
            }
            set
            {
                this.customerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> CustomerID
        {
            get
            {
                return this.customerIDField;
            }
            set
            {
                this.customerIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string SOCustomerName
        {
            get
            {
                return this.sOCustomerNameField;
            }
            set
            {
                this.sOCustomerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> SOCustomerID
        {
            get
            {
                return this.sOCustomerIDField;
            }
            set
            {
                this.sOCustomerIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumDevices
        {
            get
            {
                return this.numDevicesField;
            }
            set
            {
                this.numDevicesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumServerDevices
        {
            get
            {
                return this.numServerDevicesField;
            }
            set
            {
                this.numServerDevicesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumWorkstationDevices
        {
            get
            {
                return this.numWorkstationDevicesField;
            }
            set
            {
                this.numWorkstationDevicesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumWinServerDevices
        {
            get
            {
                return this.numWinServerDevicesField;
            }
            set
            {
                this.numWinServerDevicesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumWinWorkstationDevices
        {
            get
            {
                return this.numWinWorkstationDevicesField;
            }
            set
            {
                this.numWinWorkstationDevicesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> NumLowDiskServers
        {
            get
            {
                return this.numLowDiskServersField;
            }
            set
            {
                this.numLowDiskServersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public DeviceReportDeviceData[] Devices
        {
            get
            {
                return this.devicesField;
            }
            set
            {
                this.devicesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class DeviceIssue
    {

        private T_KeyPair[] issueField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public T_KeyPair[] Issue
        {
            get
            {
                return this.issueField;
            }
            set
            {
                this.issueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class Device
    {

        private T_KeyPair[] infoField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public T_KeyPair[] Info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "http://www.n-able.com/mickey")]
    public partial class DeviceData
    {

        private System.Nullable<int> deviceIDField;

        private System.Nullable<bool> deletedField;

        private System.Nullable<System.DateTime> installationDateField;

        private string iPAddressField;

        private string deviceClassField;

        private string oS_ReportedOSField;

        private string oS_ServicePackVersionField;

        private string oS_VersionField;

        private string computerNameField;

        private System.Nullable<long> physicalMemoryBytesField;

        private string computerSystemManufacturerField;

        private string computerSystemProductModelField;

        private string computerSystemSerialNumberField;

        private string cPUSpeedMHzField;

        private System.Nullable<int> cPUCountField;

        private DeviceHardDriveData[] hardDrivesField;

        private string customerNameField;

        private System.Nullable<int> customerIDField;

        private string sOCustomerNameField;

        private System.Nullable<int> sOCustomerIDField;

        private string nableURLField;

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> DeviceID
        {
            get
            {
                return this.deviceIDField;
            }
            set
            {
                this.deviceIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<bool> Deleted
        {
            get
            {
                return this.deletedField;
            }
            set
            {
                this.deletedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<System.DateTime> InstallationDate
        {
            get
            {
                return this.installationDateField;
            }
            set
            {
                this.installationDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string IPAddress
        {
            get
            {
                return this.iPAddressField;
            }
            set
            {
                this.iPAddressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string DeviceClass
        {
            get
            {
                return this.deviceClassField;
            }
            set
            {
                this.deviceClassField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string OS_ReportedOS
        {
            get
            {
                return this.oS_ReportedOSField;
            }
            set
            {
                this.oS_ReportedOSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string OS_ServicePackVersion
        {
            get
            {
                return this.oS_ServicePackVersionField;
            }
            set
            {
                this.oS_ServicePackVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string OS_Version
        {
            get
            {
                return this.oS_VersionField;
            }
            set
            {
                this.oS_VersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string ComputerName
        {
            get
            {
                return this.computerNameField;
            }
            set
            {
                this.computerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<long> PhysicalMemoryBytes
        {
            get
            {
                return this.physicalMemoryBytesField;
            }
            set
            {
                this.physicalMemoryBytesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string ComputerSystemManufacturer
        {
            get
            {
                return this.computerSystemManufacturerField;
            }
            set
            {
                this.computerSystemManufacturerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string ComputerSystemProductModel
        {
            get
            {
                return this.computerSystemProductModelField;
            }
            set
            {
                this.computerSystemProductModelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string ComputerSystemSerialNumber
        {
            get
            {
                return this.computerSystemSerialNumberField;
            }
            set
            {
                this.computerSystemSerialNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string CPUSpeedMHz
        {
            get
            {
                return this.cPUSpeedMHzField;
            }
            set
            {
                this.cPUSpeedMHzField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> CPUCount
        {
            get
            {
                return this.cPUCountField;
            }
            set
            {
                this.cPUCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public DeviceHardDriveData[] HardDrives
        {
            get
            {
                return this.hardDrivesField;
            }
            set
            {
                this.hardDrivesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string CustomerName
        {
            get
            {
                return this.customerNameField;
            }
            set
            {
                this.customerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> CustomerID
        {
            get
            {
                return this.customerIDField;
            }
            set
            {
                this.customerIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string SOCustomerName
        {
            get
            {
                return this.sOCustomerNameField;
            }
            set
            {
                this.sOCustomerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public System.Nullable<int> SOCustomerID
        {
            get
            {
                return this.sOCustomerIDField;
            }
            set
            {
                this.sOCustomerIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable = true)]
        public string NableURL
        {
            get
            {
                return this.nableURLField;
            }
            set
            {
                this.nableURLField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void RepositoryItemListCompletedEventHandler(object sender, RepositoryItemListCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RepositoryItemListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal RepositoryItemListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public RepositoryItem[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((RepositoryItem[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void RepositoryItemGetCompletedEventHandler(object sender, RepositoryItemGetCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RepositoryItemGetCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal RepositoryItemGetCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public RepositoryItem Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((RepositoryItem)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void RepositoryItemPublishCompletedEventHandler(object sender, RepositoryItemPublishCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RepositoryItemPublishCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal RepositoryItemPublishCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public System.DateTime Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((System.DateTime)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void AcknowledgeNotificationCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void DeviceAssetInfoExportCompletedEventHandler(object sender, DeviceAssetInfoExportCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeviceAssetInfoExportCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DeviceAssetInfoExportCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeviceData[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeviceData[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void DeviceAssetInfoExport2CompletedEventHandler(object sender, DeviceAssetInfoExport2CompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeviceAssetInfoExport2CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DeviceAssetInfoExport2CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public Device[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((Device[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void ActiveIssuesListCompletedEventHandler(object sender, ActiveIssuesListCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ActiveIssuesListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal ActiveIssuesListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeviceIssue[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeviceIssue[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void PSACredentialsValidateCompletedEventHandler(object sender, PSACredentialsValidateCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PSACredentialsValidateCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal PSACredentialsValidateCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public bool Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void CustomerDeviceReportCompletedEventHandler(object sender, CustomerDeviceReportCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CustomerDeviceReportCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal CustomerDeviceReportCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeviceReportData[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeviceReportData[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void CustomerDeviceReportGetCompletedEventHandler(object sender, CustomerDeviceReportGetCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CustomerDeviceReportGetCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal CustomerDeviceReportGetCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeviceReportData[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeviceReportData[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void CustomerListCompletedEventHandler(object sender, CustomerListCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CustomerListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal CustomerListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public Customer[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((Customer[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void CustomerListChildrenCompletedEventHandler(object sender, CustomerListChildrenCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CustomerListChildrenCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal CustomerListChildrenCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public Customer[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((Customer[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void DeviceGetCompletedEventHandler(object sender, DeviceGetCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeviceGetCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DeviceGetCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeviceInfo[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeviceInfo[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void DeviceGetStatusCompletedEventHandler(object sender, DeviceGetStatusCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeviceGetStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DeviceGetStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public DeviceStatus[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((DeviceStatus[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void DeviceListCompletedEventHandler(object sender, DeviceListCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeviceListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DeviceListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public Device[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((Device[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SOAddCompletedEventHandler(object sender, SOAddCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SOAddCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SOAddCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public int Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void CustomerAddCompletedEventHandler(object sender, CustomerAddCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CustomerAddCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal CustomerAddCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public int Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void UserAddCompletedEventHandler(object sender, UserAddCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UserAddCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal UserAddCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public int Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void JobStatusListCompletedEventHandler(object sender, JobStatusListCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class JobStatusListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal JobStatusListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public JobStatus[] Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((JobStatus[])(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void LastExportResetCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void LastExportHardResetCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SystemSetAuditorSettingsCompletedEventHandler(object sender, SystemSetAuditorSettingsCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SystemSetAuditorSettingsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SystemSetAuditorSettingsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }

        /// <remarks/>
        public T_Credentials NewCredentials
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((T_Credentials)(this.results[1]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SystemSetAuditorSettings3CompletedEventHandler(object sender, SystemSetAuditorSettings3CompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SystemSetAuditorSettings3CompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SystemSetAuditorSettings3CompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }

        /// <remarks/>
        public string SelfMonitoringACEP
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[1]));
            }
        }

        /// <remarks/>
        public T_Credentials NewCredentials
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((T_Credentials)(this.results[2]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SystemGetAuditorSettingsCompletedEventHandler(object sender, SystemGetAuditorSettingsCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SystemGetAuditorSettingsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SystemGetAuditorSettingsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public T_AuditorSettings Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((T_AuditorSettings)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SystemVerifyAuditorSettingsCompletedEventHandler(object sender, SystemVerifyAuditorSettingsCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SystemVerifyAuditorSettingsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SystemVerifyAuditorSettingsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public int Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }

        /// <remarks/>
        public string DetailedMessage
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[1]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SystemScheduleDataPushCompletedEventHandler(object sender, SystemScheduleDataPushCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SystemScheduleDataPushCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SystemScheduleDataPushCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SystemGetUserCompletedEventHandler(object sender, SystemGetUserCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SystemGetUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SystemGetUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public T_User Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((T_User)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SystemSetMonitoringTasksCompletedEventHandler(object sender, SystemSetMonitoringTasksCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SystemSetMonitoringTasksCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SystemSetMonitoringTasksCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    public delegate void SystemGetSessionIDCompletedEventHandler(object sender, SystemGetSessionIDCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SystemGetSessionIDCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SystemGetSessionIDCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public int Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }

}
