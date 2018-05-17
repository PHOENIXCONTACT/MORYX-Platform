﻿using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Global part configuration class to simplify port change
    /// </summary>
    [DataContract]
    public class WcfConfig : IConfig
    {
        /// <summary>
        /// Host for wcf services
        /// </summary>
        [DataMember]
        [DefaultValue("localhost")]
        public string Host { get; set; }

        /// <summary>
        /// Port used for http bindings
        /// </summary>
        [DataMember]
        [DefaultValue(80)]
        public int HttpPort { get; set; }

        /// <summary>
        /// Port used for net tcp bindings
        /// </summary>
        [DataMember]
        [DefaultValue(816)]
        public int NetTcpPort { get; set; }

        /// <summary>
        /// Current state of the config object. This should be decorated with the data member in order to save
        ///             the valid state after finalized configuration.
        /// </summary>
        [DataMember]
        public ConfigState ConfigState { get; set; }

        /// <summary>
        /// Gets or sets the interval of time provided for a connection to open before the
        /// transport raises an exception.
        /// </summary>
        [DataMember]
        [DefaultValue(30)]
        public int OpenTimeout { get; set; }

        /// <summary>
        /// Gets or sets the interval of time provided for a connection to close before the
        /// transport raises an exception.
        /// </summary>
        [DataMember]
        [DefaultValue(30)]
        public int CloseTimeout { get; set; }

        /// <summary>
        /// Gets or sets the interval of time provided for a write operation to complete
        /// before the transport raises an exception.
        /// </summary>
        [DataMember]
        [DefaultValue(30)]
        public int SendTimeout { get; set; }

        /// <summary>
        /// Gets or sets the interval of time that a connection can remain inactive, during
        /// which no application messages are received, before it is dropped.
        /// </summary>
        [DataMember]
        [DefaultValue(30)]
        public int ReceiveTimeout { get; set; }

        /// <summary>
        /// Exception message if load failed. This must not be decorated with a data member attribute.
        /// </summary>
        [ReadOnly(true)]
        public string LoadError { get; set; }
    }
}
