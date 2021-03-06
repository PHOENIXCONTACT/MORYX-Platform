// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Base implementation of a NetTcp based WCF service plugin
    /// </summary>
    /// <typeparam name="TConfig">The plugin configuration class</typeparam>
    /// <typeparam name="TSvcMgr">The service manager interface</typeparam>
    /// <typeparam name="TService">The service interface</typeparam>
    public abstract class BasicNetTcpConnectorPlugin<TConfig, TSvcMgr, TService> : BasicWcfConnectorPlugin<TConfig, TSvcMgr>
        where TConfig : IWcfServiceConfig
        where TSvcMgr : class, IWcfServiceManager
        where TService : class, ISessionService
    {
        /// <inheritdoc />
        public override void Start()
        {
            // Link service manager delegate
            ServiceManager.Initialize();

            // Start wcf host with the binding specific service
            Service = HostFactory.CreateHost<TService>(Config.ConnectorHost);
            Service.Start();
        }
    }
}
