using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Client;

namespace animationDemo.opcRealated.opcUA
{
    public class Subscription
    {
        #region Construction
        public Subscription()
        { }
        #endregion

        #region Fields
        /// <summary>
        /// Keeps the session. 
        /// </summary>
        private Session m_Session = null;
        /// <summary>
        /// Keeps the subscription. 
        /// </summary>
        private Opc.Ua.Client.Subscription m_Subscription = null;

        private List<object> myMonitorItemServerHandlerList = new List<object>();
        #endregion

        #region Properties
        // Session
        public Session Session
        {
            get { return m_Session; }
            set { m_Session = value; }
        }
        // Subscription
        public Opc.Ua.Client.Subscription innerSubscription
        {
            get { return m_Subscription; }
            set { m_Subscription = value; }
        }

        public List<object> ServerHandlerList
        {
            get { return myMonitorItemServerHandlerList; }
            set { myMonitorItemServerHandlerList = value; }
        }
        #endregion

        #region MonitoredItem
        /// <summary>
        /// Creates a monitored item and adds it to the existing subscription.
        /// </summary>
        /// <param name="variableNodeId">The according nodeid.</param>
        /// <param name="clientHandle">The handle of the client registering items.</param>
        /// <param name="callback">The callback to retrieve value changes.</param>
        /// <param name="samplingRate">The requested sampling rate.</param>
        /// <param name="serverHandle">The handle of the item.</param>
        public void AddDataMonitoredItem(NodeId variableNodeId, object clientHandle, valueChanged callback, uint samplingRate, out object serverHandle)
        {
            serverHandle = null;

            try
            {
                MonitoredItem monitoredItem = new MonitoredItem(m_Subscription.DefaultItem);
                ClientMonitoredItemData clientData = new ClientMonitoredItemData();
                clientData.callback = callback;
                clientData.clientHandle = clientHandle;

                // Monitored item settings:
                monitoredItem.StartNodeId = variableNodeId;
                monitoredItem.AttributeId = Attributes.Value;
                monitoredItem.MonitoringMode = MonitoringMode.Reporting;
                monitoredItem.SamplingInterval = (int)samplingRate; // Affects the read cycle between UA Server and data source
                monitoredItem.QueueSize = 1;
                monitoredItem.DiscardOldest = false;
                monitoredItem.Handle = clientData;

                // Add item to subscription.
                m_Subscription.AddItem(monitoredItem);

                // Call the server and apply any changes to the state of the subscription or monitored items.
                m_Subscription.ApplyChanges();

                // Check result of add.
                if (monitoredItem.Status.Error != null && StatusCode.IsBad(monitoredItem.Status.Error.StatusCode))
                {
                    throw ServiceResultException.Create(
                        monitoredItem.Status.Error.StatusCode.Code,
                        "Creation of data monitored item failed");
                }

                serverHandle = monitoredItem;
                myMonitorItemServerHandlerList.Add(serverHandle);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Modifies a monitored item.
        /// </summary>
        public void ModifyMonitoredItem(object serverHandle)
        {
            try
            {
                m_Subscription.ApplyChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Removes a monitored item.
        /// </summary>
        public void RemoveMonitoredItem(object serverHandle)
        {
            try
            {
                m_Subscription.RemoveItem((MonitoredItem)serverHandle);
                m_Subscription.ApplyChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }

    public class ClientMonitoredItemData
    {
        public object clientHandle = null;
        public valueChanged callback = null;
    }
}
