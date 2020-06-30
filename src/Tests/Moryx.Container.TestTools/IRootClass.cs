// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Modules;

namespace Marvin.Container.TestTools
{
    public interface IRootClass : IConfiguredPlugin<RootClassFactoryConfig>
    {
        IConfiguredComponent ConfiguredComponent { get; set; }
        string GetName();
    }
}