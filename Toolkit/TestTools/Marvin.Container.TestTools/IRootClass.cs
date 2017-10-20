﻿using Marvin.Modules.ModulePlugins;

namespace Marvin.Container.TestTools
{
    public interface IRootClass : IConfiguredModulePlugin<RootClassFactoryConfig>
    {
        IConfiguredComponent ConfiguredComponent { get; set; }
        string GetName();
    }
}