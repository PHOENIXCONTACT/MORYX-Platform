﻿using System.Runtime.Serialization;
using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    [DataContract]
    internal class ModuleMaintenanceConfig : MaintenancePluginConfig
    {
        public ModuleMaintenanceConfig()
        {
            ProvidedEndpoint = new HostConfig
            {
                BindingType = ServiceBindingType.WebHttp,
                Endpoint = "ModuleMaintenance",
                MetadataEnabled = true
            };
        }

        public override string PluginName => ModuleMaintenancePlugin.PluginName;
    }
}