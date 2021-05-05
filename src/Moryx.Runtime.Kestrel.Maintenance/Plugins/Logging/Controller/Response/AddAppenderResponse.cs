// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Kestrel.Maintenance
{
    /// <summary>
    /// Response for adding a logging appender
    /// </summary>
    public class AddAppenderResponse
    {
        /// <summary>
        /// Id of the logging appender
        /// </summary>
        public int AppenderId { get; set; }
    }
}
