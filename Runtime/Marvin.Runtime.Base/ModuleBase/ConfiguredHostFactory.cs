﻿using Marvin.Logging;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Base
{
    internal class ConfiguredHostFactory : IConfiguredHostFactory
    {
        /// Injected properties
        public IModuleLogger Logger { get; set; }
        public ITypedHostFactory Factory { get; set; }

        private readonly IWcfHostFactory _hostFactory;
        public ConfiguredHostFactory(IWcfHostFactory hostFactory)
        {
            _hostFactory = hostFactory;
        }

        public IConfiguredServiceHost CreateHost<T>(HostConfig config)
        {
            return _hostFactory.CreateHost<T>(config, Factory, Logger);
        }
    }
}