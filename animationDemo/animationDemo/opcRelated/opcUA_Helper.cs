#region Copyright
//------------------------------------------------------------------------
//   OPC UA - Helper
//
//   Copyright (C) Siemens AG 2008  All Rights Reserved. Confidential
//------------------------------------------------------------------------
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Opc.Ua;

namespace animationDemo.opcRealated.opcUA
{
    public partial class opcUA_Helper
    {
        /// <summary>
        /// Creates a minimal application configuration for a client.
        /// 为OPC客户端创建一个配置程序
        /// </summary>
        /// <remarks>
        /// In most cases the application configuration will be loaded from an XML file. 
        /// This example populates the configuration in code.
        /// 在大多数情况下，OPC应用会从XML文件中读取相应的配置信息，
        /// 本程序从代码中直接进行配置
        /// </remarks>
        public static ApplicationConfiguration CreateClientConfiguration()
        {
            // The application configuration can be loaded from any file.
            // ApplicationConfiguration.Load() method loads configuration by looking up a file path in the App.config.
            // This approach allows applications to share configuration files and to update them.
            ApplicationConfiguration configuration = new ApplicationConfiguration();

            // Step 1 - Specify the server identity.
            configuration.ApplicationName = "625IIS App";
            configuration.ApplicationType = ApplicationType.Client;
            configuration.ApplicationUri = "urn:625-PC:625IIS.IIS.V1:625IIS softWare";
            configuration.ProductUri = "urn:625IIS.625IIS.V1:625IIS softWare";

            configuration.SecurityConfiguration = new SecurityConfiguration();

            // Step 2 - Specify the server's application instance certificate.
            //第二步-定义服务器的应用实例证书

            // Application instance certificates must be placed in a windows certficate store because that is 
            // the best way to protect the private key. Certificates in a store are identified with 4 parameters:
            // StoreLocation, StoreName, SubjectName and Thumbprint.
            //
            // In this example the following values are used:
            // 
            //   LocalMachine    - use the machine wide certificate store.
            //   Personal        - use the store for individual certificates.
            //   ApplicationName - use the application name as a search key.   

            configuration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier();
            configuration.SecurityConfiguration.ApplicationCertificate.StoreType = CertificateStoreType.Windows;
            configuration.SecurityConfiguration.ApplicationCertificate.StorePath = "LocalMachine\\My";
            configuration.SecurityConfiguration.ApplicationCertificate.SubjectName = configuration.ApplicationName;

            // trust all applications installed on the same machine.
            //信任所有在本机上运行的应用程序
            configuration.SecurityConfiguration.TrustedPeerCertificates.StoreType = CertificateStoreType.Windows;
            configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath = "LocalMachine\\My";

            // find the certificate in the store.
            X509Certificate2 clientCertificate = configuration.SecurityConfiguration.ApplicationCertificate.Find(true);

            // create a new certificate if one not found.
            if (clientCertificate == null)
            {
                // this code would normally be called as part of the installer - called here to illustrate.
                // create a new certificate an place it in the LocalMachine/Personal store.
                clientCertificate = CertificateFactory.CreateCertificate(
                    configuration.SecurityConfiguration.ApplicationCertificate.StoreType,
                    configuration.SecurityConfiguration.ApplicationCertificate.StorePath,
                    configuration.ApplicationUri,
                    configuration.ApplicationName,
                    null,
                    null,
                    1024,
                    12);
            }

            // Step 3 - Specify the supported transport configurations.

            // The SDK requires Binding objects which are sub-types of Opc.Ua.Bindings.BaseBinding
            // These two lines add support for SOAP/HTTP w/ WS-* and UA-TCP. Support for other protocols 
            // such as .NET TCP can be added but they would not be considered interoperable across different vendors. 
            // Only one binding per URL scheme is allowed.
            configuration.TransportConfigurations.Add(new TransportConfiguration(Utils.UriSchemeOpcTcp, typeof(Opc.Ua.Bindings.UaTcpBinding)));
            configuration.TransportConfigurations.Add(new TransportConfiguration(Utils.UriSchemeHttp, typeof(Opc.Ua.Bindings.UaSoapXmlBinding)));

            // Step 4 - Specify the supported transport quotas.

            // The transport quotas are used to set limits on the contents of messages and are
            // used to protect against DOS attacks and rogue clients. They should be set to
            // reasonable values.
            configuration.TransportQuotas = new TransportQuotas();
            configuration.TransportQuotas.OperationTimeout = 360000;
            configuration.TransportQuotas.MaxStringLength = 67108864;
            configuration.ServerConfiguration = new ServerConfiguration();

            // Step 5 - Specify the client specific configuration.
            configuration.ClientConfiguration = new ClientConfiguration();
            configuration.ClientConfiguration.DefaultSessionTimeout = 360000;

            // Step 6 - Validate the configuration.

            // This step checks if the configuration is consistent and assigns a few internal variables
            // that are used by the SDK. This is called automatically if the configuration is loaded from
            // a file using the ApplicationConfiguration.Load() method.          
            configuration.Validate(ApplicationType.Client);

            return configuration;
        }

        /// <summary>
        /// Creates a minimal endpoint description which allows a client to connect to a server.
        /// </summary>
        /// <remarks>
        /// In most cases the client will use the server's discovery endpoint to fetch the information
        /// constained in this structure.
        /// </remarks>
        public static EndpointDescription CreateEndpointDescription(string Url)
        {
            // create the endpoint description.
            EndpointDescription endpointDescription = new EndpointDescription();

            //endpointDescription.EndpointUrl = Utils.Format("opc.tcp://{0}:4841", System.Net.Dns.GetHostName());
            endpointDescription.EndpointUrl = Url;

            // specify the security policy to use.
            //endpointDescription.SecurityPolicyUri = SecurityPolicies.Basic128Rsa15;
            endpointDescription.SecurityPolicyUri = SecurityPolicies.None;
            //endpointDescription.SecurityMode      = MessageSecurityMode.SignAndEncrypt;
            endpointDescription.SecurityMode = MessageSecurityMode.None;

            // specify the transport profile.
            endpointDescription.TransportProfileUri = Profiles.WsHttpXmlOrBinaryTransport;

            // load the the server certificate from the local certificate store.
            CertificateIdentifier certificateIdentifier = new CertificateIdentifier();

            certificateIdentifier.StoreType = CertificateStoreType.Windows;
            certificateIdentifier.StorePath = "LocalMachine\\My";

            certificateIdentifier.SubjectName = "625IIS App";

            X509Certificate2 serverCertificate = certificateIdentifier.Find();

            if (serverCertificate == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadCertificateInvalid, "Could not find server certificate: {0}", certificateIdentifier.SubjectName);
            }

            endpointDescription.ServerCertificate = serverCertificate.GetRawCertData();

            return endpointDescription;
        }
    }
}
