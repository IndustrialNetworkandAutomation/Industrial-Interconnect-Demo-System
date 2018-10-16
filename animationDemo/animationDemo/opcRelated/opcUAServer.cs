using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

using System.Threading;         //引入多线程
using Opc.Ua;
using Opc.Ua.Client;


namespace animationDemo.opcRealated.opcUA
{
    /// <summary>
    /// Event handler for the event that the client received a value change.
    /// </summary>
    public delegate void valueChanged(object clientHandle, DataValue value);

    /// <summary>
    /// Event handler for the event that the client received a untrusted server certificate.
    /// </summary>
    public delegate void certificateValidation(CertificateValidator validator, CertificateValidationEventArgs e);

    /// <summary>
    /// 该类封装了OPC UA服务器类并且提供了一些与服务器相关的方法
    /// </summary>
    public class opcUAServer
    {
        #region Construction
        public opcUAServer()
        {
            //m_DataManager = new dataManager();
        }
        #endregion

        #region Fields
        /// <summary> 
        /// 为OPC UA服务器创建唯一标识 
        /// </summary>
        private String m_ServerID = null;

        /// <summary>
        /// 为OPC UA服务器创建服务器名
        /// </summary>
        private String m_ServerName = null;
        /// <summary>
        /// 是否为设备参数
        /// </summary>
        private String if_Machine = null;

        /// <summary> 
        /// Keeps a session with an UA server. 
        /// </summary>
        private Session m_Session = null;

        /// <summary> 
        /// Interface which encapsulates the use of the browse service of an UA server. 
        /// </summary>
        private Browser m_Browser = null;

        /// <summary> 
        /// Keeps a hash table for attribute names. 
        /// </summary>
        private Hashtable m_hashAttributeNames = null;

        public List<Subscription> curSubscriptionList = new List<Subscription>();

        /// <summary>
        /// 每个Server添加一条线程，用以监控自身的变量
        /// </summary>
        private Thread m_Thread = null;

        //private dataManager m_DataManager = null;
        #endregion
                
        #region Properties
        /// <summary>
        /// Use the certificateValidation delegate as event.
        /// </summary>
        public event certificateValidation CertificateEvent = null;

        /// <summary>
        /// Provides the session being established with an OPC UA server.
        /// </summary>
        public Session Session
        {
            get { return m_Session; }
        }

        //public dataManager DataManager
        //{
        //    get { return m_DataManager; }
        //}

        /// <summary>
        /// 新增数据值
        /// </summary>
        /// <param name="valueKey"></param>
        /// <param name="value"></param>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public Boolean addDataValue(String valueKey, String value, String timeStamp, object serverHandler)
        {
            try
            {
                //this.m_DataManager.addUAValue(valueKey, value, timeStamp, serverHandler, m_ServerID);

            }
            catch (Exception ex)
            {
                Console.WriteLine("server内部的添加出现异常，异常信息：" + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 修改数据值
        /// </summary>
        /// <param name="valueKey"></param>
        /// <param name="value"></param>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public Boolean updateDataValue(String valueKey, String value, String timeStamp)
        {
            try
            {
                //this.m_DataManager.updateValue(valueKey, value, timeStamp);

            }
            catch (Exception ex)
            {
                Console.WriteLine("server内部的添加出现异常，异常信息：" + ex.Message);
                return false;
            }
            return true;
        }
        #endregion
             
        #region Connect
        /// <summary>Establishes the connection to an OPC UA server.</summary>
        /// <param name="url">The Url of the endpoint.</param>
        /// <returns>Result code.</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Connect(string Url)
        {
            try
            {
                // Create the configuration.     
                ApplicationConfiguration configuration = opcUA_Helper.CreateClientConfiguration();

                // Create the endpoint description.
                EndpointDescription endpointDescription = opcUA_Helper.CreateEndpointDescription(Url);

                // Create the endpoint configuration (use the application configuration to provide default values).
                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);

                // The default timeout for a requests sent using the channel.
                endpointConfiguration.OperationTimeout = 300000;

                // Use the pure binary encoding on the wire.
                endpointConfiguration.UseBinaryEncoding = true;

                // Create the endpoint.
                ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                // Create the binding factory.
                BindingFactory bindingFactory = BindingFactory.Create(configuration);

                // Update endpoint description using the discovery endpoint.
                if (endpoint.UpdateBeforeConnect)
                {
                    endpoint.UpdateFromServer(bindingFactory); 
                    endpointDescription = endpoint.Description;
                    endpointConfiguration = endpoint.Configuration;
                }

                X509Certificate2 clientCertificate = configuration.SecurityConfiguration.ApplicationCertificate.Find();

                // Set up a callback to handle certificate validation errors.
                configuration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

                // Initialize the channel which will be created with the server.
                SessionChannel channel = SessionChannel.Create(
                    configuration,
                    endpointDescription,
                    endpointConfiguration,
                    bindingFactory,
                    clientCertificate,
                    null);

                // Wrap the channel with the session object.
                // This call will fail if the server does not trust the client certificate.
                m_Session = new Session(channel, configuration, endpoint);
                m_Session.ReturnDiagnostics = DiagnosticsMasks.All;

                // Register keep alive callback.
                m_Session.KeepAlive += new KeepAliveEventHandler(Session_KeepAlive);

                UserIdentity identity = new UserIdentity();
                

                // Create the session. This actually connects to the server.
                // Passing null for the user identity will create an anonymous session.
                m_Session.Open("625IIS - main session", identity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 建立与OPC UA的连接，该种方式需要用户名与密码形式
        /// </summary>
        /// <param name="Url">需要连接的Url地址信息</param>
        /// <param name="userName">用户名</param>
        /// <param name="userPwd">密码</param>
        public void Connect(string Url, String userName, String userPwd)
        {
            try
            {
                // Create the configuration.     
                ApplicationConfiguration configuration = opcUA_Helper.CreateClientConfiguration();

                // Create the endpoint description.
                EndpointDescription endpointDescription = opcUA_Helper.CreateEndpointDescription(Url);

                // Create the endpoint configuration (use the application configuration to provide default values).
                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);

                // The default timeout for a requests sent using the channel.
                endpointConfiguration.OperationTimeout = 300000;

                // Use the pure binary encoding on the wire.
                endpointConfiguration.UseBinaryEncoding = true;

                // Create the endpoint.
                ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                // Create the binding factory.
                BindingFactory bindingFactory = BindingFactory.Create(configuration);

                // Update endpoint description using the discovery endpoint.
                if (endpoint.UpdateBeforeConnect)
                {
                    endpoint.UpdateFromServer(bindingFactory);
                    endpointDescription = endpoint.Description;
                    endpointConfiguration = endpoint.Configuration;
                }

                X509Certificate2 clientCertificate = configuration.SecurityConfiguration.ApplicationCertificate.Find();

                // Set up a callback to handle certificate validation errors.
                configuration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

                // Initialize the channel which will be created with the server.
                SessionChannel channel = SessionChannel.Create(
                    configuration,
                    endpointDescription,
                    endpointConfiguration,
                    bindingFactory,
                    clientCertificate,
                    null);

                // Wrap the channel with the session object.
                // This call will fail if the server does not trust the client certificate.
                m_Session = new Session(channel, configuration, endpoint);
                m_Session.ReturnDiagnostics = DiagnosticsMasks.All;

                // Register keep alive callback.
                m_Session.KeepAlive += new KeepAliveEventHandler(Session_KeepAlive);

                UserIdentity identity = new UserIdentity(userName, userPwd);


                // Create the session. This actually connects to the server.
                // Passing null for the user identity will create an anonymous session.
                m_Session.Open("625IIS - main session", identity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>Establishes the connection to an OPC UA server.</summary>
        /// <param name="url">The Url of the endpoint.</param>
        /// <returns>Result code.</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Connect(EndpointDescription endpointDescription)
        {
            try
            {
                // Create the configuration.     
                ApplicationConfiguration configuration = opcUA_Helper.CreateClientConfiguration();

                // Create the endpoint configuration (use the application configuration to provide default values).
                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);

                // The default timeout for a requests sent using the channel.
                endpointConfiguration.OperationTimeout = 300000;

                // Use the pure binary encoding on the wire.
                endpointConfiguration.UseBinaryEncoding = true;

                // Create the endpoint.
                ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                // Create the binding factory.
                BindingFactory bindingFactory = BindingFactory.Create(configuration);

                X509Certificate2 clientCertificate = configuration.SecurityConfiguration.ApplicationCertificate.Find();

                // Set up a callback to handle certificate validation errors.
                configuration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

                // Initialize the channel which will be created with the server.
                SessionChannel channel = SessionChannel.Create(
                    configuration,
                    endpointDescription,
                    endpointConfiguration,
                    bindingFactory,
                    clientCertificate,
                    null);

                // Wrap the channel with the session object.
                // This call will fail if the server does not trust the client certificate.
                m_Session = new Session(channel, configuration, endpoint);
                m_Session.ReturnDiagnostics = DiagnosticsMasks.All;

                // Register keep alive callback.
                m_Session.KeepAlive += new KeepAliveEventHandler(Session_KeepAlive);

                UserIdentity identity = new UserIdentity();

                // Create the session. This actually connects to the server.
                // Passing null for the user identity will create an anonymous session.
                m_Session.Open("Siemens OPC UA sample client - main session", identity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
                
        /// <summary>Releases the connection to an OPC UA server.</summary>
        /// <returns>Result code.</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public int Disconnect()
        {
            int result = 0;
            
            // Close the session.
            try
            {
                m_Session.Close(10000);
            }
            catch (Exception e)
            {
                return -1;
                throw e;
            } 
           
            // Delete browser.
            m_Browser = null;
            
            return result;
        }
        #endregion 
        
        #region Browse
        /// <summary>
        /// Navigates through the address space of an OPC UA server.
        /// </summary>
        /// <param name="nodeId">The starting node.</param>
        /// <param name="browseResults">A list of results for the passed starting node and filters.</param>
        /// <returns>Result code.</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public int Browse(NodeId nodeId, out ReferenceDescriptionCollection browseResults)
        {
            int result = 0;
            
            if (m_Browser == null)
            {
                // Instantiate browser.
                m_Browser = new Browser(m_Session);
            }

            // Browse filter: only follow hierarchical references.
            m_Browser.ReferenceTypeId = ReferenceTypes.HierarchicalReferences;

            browseResults = null;

            try
            {
                // The actual call to the server.
                browseResults = m_Browser.Browse(nodeId);
            }
            catch (Exception e)
            {
                result = -1;
                throw e;
            }

            return result;
        }
        #endregion 

        #region Read / Write
        /// <summary>
        /// Reads any attribute of one or more nodes.
        /// </summary>
        /// <param name="nodesToRead">A list of nodes to read.</param>
        /// <param name="results">A list of read results.</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Read(
            ReadValueIdCollection nodesToRead,
            out DataValueCollection results)
        {
            results = null;
            DiagnosticInfoCollection diagnosticInfos = null;

            try
            {
                // The actual call to the server.
                m_Session.Read(
                    null,
                    0,
                    TimestampsToReturn.Both,
                    nodesToRead,
                    out results,
                    out diagnosticInfos);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Reads the value attribute of one or more nodes.
        /// </summary>
        /// <param name="nodesToRead">A list of nodes to read.</param>
        /// <param name="results">A list of data values.</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public ResponseHeader ReadValues(
            NodeIdCollection nodesToRead,
            out DataValueCollection results)
        {
            ResponseHeader response = null;

            results = null;
            DiagnosticInfoCollection diagnosticInfos;

            // build list of attributes to read.
            ReadValueIdCollection valueIdsToRead = new ReadValueIdCollection();

            try
            {
                for (int i = 0; i < nodesToRead.Count; i++)
                {
                    ReadValueId attributeToRead = new ReadValueId();
                    attributeToRead.NodeId = nodesToRead[i];
                    attributeToRead.AttributeId = Attributes.Value;
                    attributeToRead.Handle = attributeIdToString(Attributes.Value);
                    valueIdsToRead.Add(attributeToRead);
                }

                response = m_Session.Read(
                        null,
                        0,
                        TimestampsToReturn.Both,
                        valueIdsToRead,
                        out results,
                        out diagnosticInfos);
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }
        
        /// <summary>
        /// Writes the Value attribute of one or more nodes.
        /// </summary>
        /// <param name="nodesToWrite">A list of nodes to write.</param>
        /// <param name="results">A list of values to write.</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void WriteValues(
            NodeIdCollection nodesToWrite,
            DataValueCollection values,
            out StatusCodeCollection results)
        {
            results = null;
            
            DiagnosticInfoCollection diagnosticInfos = null;

            // Build list of attributes to write.
            WriteValueCollection valuesToWrite = new WriteValueCollection();

            try
            {
                for (int i = 0; i < nodesToWrite.Count; i++)
                {
                    WriteValue attributeToWrite = new WriteValue();
                    attributeToWrite.NodeId = nodesToWrite[i];
                    // We have to write the Value attribute only:
                    attributeToWrite.AttributeId = Attributes.Value;
                    attributeToWrite.Handle = attributeIdToString(Attributes.Value);
                    // Set the according value:
                    attributeToWrite.Value = values[i];
                    valuesToWrite.Add(attributeToWrite);
                }

                // Actual call to server.
                m_Session.Write(
                        null,
                        valuesToWrite,
                        out results,
                        out diagnosticInfos);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region Subscribe for Data Changes
        /// <summary>
        /// Creates a subscription and associate it with the current session.
        /// </summary>
        /// <param name="publishingInterval">The requested publishing interval.</param>
        /// <returns>Instance of class Subscription.</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public Subscription AddSubscription(int publishingInterval)
        {
            Subscription newSubscription = null;
            
            try
            {
                Opc.Ua.Client.Subscription innerSubscription = new Opc.Ua.Client.Subscription(m_Session.DefaultSubscription);

                innerSubscription.DisplayName = "My Subscription Name";
                innerSubscription.PublishingEnabled = true;
                innerSubscription.PublishingInterval = publishingInterval; // in milliseconds.
                innerSubscription.KeepAliveCount = 10;   // 10*UaRefreshRate  = 5s if UaRefreshRate = 500
                innerSubscription.LifetimeCount = 100;  // UaRefreshRate*100 = 50s if UaRefreshRate = 500;
                innerSubscription.MaxNotificationsPerPublish = 100;

                // Associate the subscription with the session.            
                m_Session.AddSubscription(innerSubscription);

                // Call the server and create the subscription.
                innerSubscription.Create();

                // At this point the subscription is sending publish requests at the keep alive rate.
                // Use the Notification event the session to receive updates when a publish completes.
                m_Session.Notification += new NotificationEventHandler(Session_Notification);

                newSubscription = new Subscription();
                newSubscription.Session = m_Session;
                newSubscription.innerSubscription = innerSubscription;
            }
            catch (Exception e)
            {
                throw e;
            }

            return newSubscription;
        }

        // Modifies the publishing interval of a subscription.
        /// <summary>
        /// Creates a subscription and associates it with the current session.
        /// </summary>
        /// <param name="subscription">The subscription object.</param>
        /// <param name="subscription">The newly requested publishing interval.</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void ModifySubscription(Subscription subscription,int publishingInterval)
        {
            try
            {
                // Modify publishing interval
                subscription.innerSubscription.PublishingInterval = publishingInterval;

                // Apply change
                subscription.innerSubscription.Modify();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Removes a subscription from the session.
        /// </summary>
        /// <param name="subscription">The subscription object.</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void RemoveSubscription(Subscription subscription)
        {
            try
            {
                // Remove the subscription from the corresponding session.
                m_Session.RemoveSubscription(subscription.innerSubscription);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Will be raised when a certificate is classified as untrusted.
        /// </summary>
        /// <param name="validator">The event handler.</param>
        /// <param name="e">The <see cref="Opc.Ua.CertificateValidationEventArgs"/>Instance containing the event data.</param>
        private void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (CertificateEvent != null)
            {
                CertificateEvent(validator, e);
            }
            else
            {
                e.Accept = false;
            }
        }

        /// <summary>
        /// Keep-alive callback.
        /// </summary>
        /// <param name="session">The target of the event.</param>
        /// <param name="e">The <see cref="Opc.Ua.KeepAliveEventArgs"/>Instance containing the event data.</param>
        private void Session_KeepAlive(Session session, KeepAliveEventArgs e)
        {
        }

        /// <summary>
        /// Raised when a publish response arrives from the server.
        /// </summary>
        /// <param name="session">The target of the event.</param>
        /// <param name="e">The <see cref="Opc.Ua.Client.NotifacationEventArgs"/>Instance containing the event data.</param> 
        private void Session_Notification(Session session, NotificationEventArgs e)
        {
            NotificationMessage message = e.NotificationMessage;

            // Check for keep alive.
            if (message.NotificationData.Count == 0)
            {
                return;
            }

            // Get the data changes (oldest to newest).
            foreach (MonitoredItemNotification datachange in message.GetDataChanges(false))
            {
                // Lookup the monitored item.
                MonitoredItem monitoredItem = e.Subscription.FindItemByClientHandle(datachange.ClientHandle);

                if (monitoredItem == null)
                {
                    continue;
                }

                ClientMonitoredItemData clientData = monitoredItem.Handle as ClientMonitoredItemData;

                clientData.callback(clientData.clientHandle, datachange.Value);
            }
        }
        #endregion
                
        #region Helper Methods
        /// <summary>
        /// Converts an attribute id to string.
        /// </summary>
        /// <param name="attributeId">The attribute id.</param>
        /// <returns>The attribute name as string.</returns>
        public string attributeIdToString(uint attributeId)
        {
            // Populate hashtable if called for the first time
            if (m_hashAttributeNames == null)
            {
                m_hashAttributeNames = new Hashtable();

                m_hashAttributeNames.Add(Attributes.AccessLevel, "AccessLevel");
                m_hashAttributeNames.Add(Attributes.ArrayDimensions, "ArrayDimensions");
                m_hashAttributeNames.Add(Attributes.BrowseName, "BrowseName");
                m_hashAttributeNames.Add(Attributes.ContainsNoLoops, "ContainsNoLoops");
                m_hashAttributeNames.Add(Attributes.DataType, "DataType");
                m_hashAttributeNames.Add(Attributes.Description, "Description");
                m_hashAttributeNames.Add(Attributes.DisplayName, "DisplayName");
                m_hashAttributeNames.Add(Attributes.EventNotifier, "EventNotifier");
                m_hashAttributeNames.Add(Attributes.Executable, "Executable");
                m_hashAttributeNames.Add(Attributes.Historizing, "Historizing");
                m_hashAttributeNames.Add(Attributes.InverseName, "InverseName");
                m_hashAttributeNames.Add(Attributes.IsAbstract, "IsAbstract");
                m_hashAttributeNames.Add(Attributes.MinimumSamplingInterval, "MinimumSamplingInterval");
                m_hashAttributeNames.Add(Attributes.NodeClass, "NodeClass");
                m_hashAttributeNames.Add(Attributes.NodeId, "NodeId");
                m_hashAttributeNames.Add(Attributes.Symmetric, "Symmetric");
                m_hashAttributeNames.Add(Attributes.UserAccessLevel, "UserAccessLevel");
                m_hashAttributeNames.Add(Attributes.UserExecutable, "UserExecutable");
                m_hashAttributeNames.Add(Attributes.UserWriteMask, "UserWriteMask");
                m_hashAttributeNames.Add(Attributes.Value, "Value");
                m_hashAttributeNames.Add(Attributes.ValueRank, "ValueRank");
                m_hashAttributeNames.Add(Attributes.WriteMask, "WriteMask");       
            }

            string ret = (string)m_hashAttributeNames[attributeId];

            return ret;
        }

        // 
        /// <summary>
        /// Converts an attribute value to an user readable string.
        /// </summary>
        /// <param name="nodeRead">Contains the attriute id.</param>
        /// <param name="nodeRead">The value to convert.</param>
        /// <returns>The attribute value as string.</returns>
        public string attributeValueToString(ReadValueId nodeRead, DataValue result)
        {
            if (result == null || result.Value == null)
            {
                return "";
            }

            string ret = "";
            switch (nodeRead.AttributeId)
            {
                case Attributes.AccessLevel:
                case Attributes.UserAccessLevel:
                    {
                        // Check datatype.
                        if (result.Value.GetType() == typeof(byte))
                        {
                            if (((byte)result.Value & AccessLevels.CurrentRead) == AccessLevels.CurrentRead)
                            {
                                ret = "Readable";
                            }
                            if (((byte)result.Value & AccessLevels.CurrentWrite) == AccessLevels.CurrentWrite)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "Writeable";
                            }
                            if (((byte)result.Value & AccessLevels.HistoryRead) == AccessLevels.HistoryRead)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "HistoryReadable";
                            }
                            if (((byte)result.Value & AccessLevels.HistoryWrite) == AccessLevels.HistoryWrite)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "HistoryWriteable";
                            }
                            if (ret.Length == 0)
                            {
                                ret = "None";
                            }
                        }
                        else
                        {
                            ret = "Invalid Datatype: Expected Byte, Received " + result.Value.GetType().ToString();
                        }
                    }
                    break;

                case Attributes.WriteMask:
                case Attributes.UserWriteMask:
                    {
                        // Check datatype.
                        if (result.Value.GetType() == typeof(uint))
                        {
                            if (((uint)result.Value & (uint)AttributeWriteMask.AccessLevel) == (uint)AttributeWriteMask.AccessLevel)
                            {
                                ret = "AccessLevel";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.BrowseName) == (uint)AttributeWriteMask.BrowseName)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "BrowseName";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.ContainsNoLoops) == (uint)AttributeWriteMask.ContainsNoLoops)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "ContainsNoLoops";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.DataType) == (uint)AttributeWriteMask.DataType)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "DataType";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.Description) == (uint)AttributeWriteMask.Description)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "Description";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.DisplayName) == (uint)AttributeWriteMask.DisplayName)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "DisplayName";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.EventNotifier) == (uint)AttributeWriteMask.EventNotifier)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "EventNotifier";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.Executable) == (uint)AttributeWriteMask.Executable)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "Executable";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.Historizing) == (uint)AttributeWriteMask.Historizing)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "Historizing";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.InverseName) == (uint)AttributeWriteMask.InverseName)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "InverseName";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.IsAbstract) == (uint)AttributeWriteMask.IsAbstract)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "IsAbstract";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.MinimumSamplingInterval) == (uint)AttributeWriteMask.MinimumSamplingInterval)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "MinimumSamplingInterval";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.Symmetric) == (uint)AttributeWriteMask.Symmetric)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "Symmetric";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.UserAccessLevel) == (uint)AttributeWriteMask.UserAccessLevel)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "UserAccessLevel";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.UserExecutable) == (uint)AttributeWriteMask.UserExecutable)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "UserExecutable";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.UserWriteMask) == (uint)AttributeWriteMask.UserWriteMask)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "UserWriteMask";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.ValueRank) == (uint)AttributeWriteMask.ValueRank)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "ValueRank";
                            }
                            if (((uint)result.Value & (uint)AttributeWriteMask.WriteMask) == (uint)AttributeWriteMask.WriteMask)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "WriteMask";
                            }
                            if (ret.Length == 0)
                            {
                                ret = "None";
                            }
                        }
                        else
                        {
                            ret = "Invalid Datatype: Expected Uint32, Received " + result.Value.GetType().ToString();
                        }
                    }
                    break;

                case Attributes.NodeClass:
                    {
                        // Check datatype.
                        if (result.Value.GetType() == typeof(Int32))
                        {
                            switch ((Int32)result.Value)
                            {
                                case (int)NodeClass.DataType:
                                    ret = "DataType";
                                    break;
                                case (int)NodeClass.Method:
                                    ret = "Method";
                                    break;
                                case (int)NodeClass.Object:
                                    ret = "Object";
                                    break;
                                case (int)NodeClass.ObjectType:
                                    ret = "ObjectType";
                                    break;
                                case (int)NodeClass.ReferenceType:
                                    ret = "ReferenceType";
                                    break;
                                case (int)NodeClass.Unspecified:
                                    ret = "Unspecified";
                                    break;
                                case (int)NodeClass.Variable:
                                    ret = "Variable";
                                    break;
                                case (int)NodeClass.VariableType:
                                    ret = "VariableType";
                                    break;
                                case (int)NodeClass.View:
                                    ret = "View";
                                    break;
                                default:
                                    ret = "Invalid NodeClass";
                                    break;
                            }
                        }
                        else
                        {
                            ret = "Invalid Datatype: Expected Int32, Received " + result.Value.GetType().ToString();
                        }
                    }
                    break;

                case Attributes.EventNotifier:
                    {
                        // Check datatype.
                        if (result.Value.GetType() == typeof(byte))
                        {
                            if (((byte)result.Value & EventNotifiers.HistoryRead) == EventNotifiers.HistoryRead)
                            {
                                ret = "HistoryRead";
                            }
                            if (((byte)result.Value & EventNotifiers.HistoryWrite) == EventNotifiers.HistoryWrite)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "HistoryWrite";
                            }
                            if (((byte)result.Value & EventNotifiers.SubscribeToEvents) == EventNotifiers.SubscribeToEvents)
                            {
                                if (ret.Length > 0)
                                {
                                    ret += ", ";
                                }
                                ret += "SubscribeToEvents";
                            }
                            if (ret.Length == 0)
                            {
                                ret += "None";
                            }
                        }
                        else
                        {
                            ret = "Invalid Datatype: Expected Byte, Received " + result.Value.GetType().ToString();
                        }
                    }
                    break;
                case Attributes.DataType:
                    {
                        // Check datatype.
                        if (result.Value.GetType() == typeof(NodeId))
                        {
                            NodeId node = (NodeId)result.Value;
                            INode datatype = m_Session.NodeCache.Find(node);

                            if (datatype != null)
                            {
                                ret = String.Format("{0}", datatype);
                            }
                            else
                            {
                                ret = String.Format("{0}", node);
                            }
                        }
                        else
                        {
                            ret = "Invalid Datatype: Expected NodeId, Received " + result.Value.GetType().ToString();
                        }                       

                    }
                    break;

                case Attributes.ArrayDimensions:
                case Attributes.BrowseName:
                case Attributes.ContainsNoLoops:
                case Attributes.Description:
                case Attributes.DisplayName:
                case Attributes.Executable:
                case Attributes.Historizing:
                case Attributes.InverseName:
                case Attributes.IsAbstract:
                case Attributes.MinimumSamplingInterval:
                case Attributes.NodeId:
                case Attributes.Symmetric:
                case Attributes.UserExecutable:
                case Attributes.Value:
                case Attributes.ValueRank:

                default:
                    ret = result.Value.ToString();
                    break;
            }
            return ret;
        }
        #endregion

        #region userDefine
        public String getServerID()
        {
            return this.m_ServerID;
        }

        public String getServerName()
        {
            return this.m_ServerName;
        }

        public String getIfMachine()
        {
            return this.if_Machine;
        }

        public void setServerID(String curServerID)
        {
            this.m_ServerID = curServerID;
        }

        public void setServerName(String curServerName)
        {
            this.m_ServerName = curServerName;
        }

        public void setIfMachine(String if_Machine)
        {
            this.if_Machine = if_Machine;
        }

        //public void startInqueue(List<String> powerData)
        //{
        //    this.m_DataManager.initQueueList(powerData);
        //}

        public Subscription addSubscriptionList(int PublishingInterval)
        {
            Subscription curSubscription = new Subscription();
            curSubscription = this.AddSubscription(PublishingInterval);
            curSubscriptionList.Add(curSubscription);

            return curSubscription;
        }

        //public void removeAllItem()
        //{
        //    if (m_DataManager != null)
        //    {
        //        m_DataManager.removeAllValue();
        //    }
        //}
        #endregion
    }
}
