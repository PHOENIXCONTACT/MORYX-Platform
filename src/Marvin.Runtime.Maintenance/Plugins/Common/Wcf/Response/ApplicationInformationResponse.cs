﻿namespace Marvin.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Response contract for application information
    /// </summary>
    public class ApplicationInformationResponse
    {
        /// <summary>
        /// Product name of this application
        /// </summary>
        public string AssemblyProduct { get; set; }

        /// <summary>
        /// Product version of this application
        /// </summary>
        public string AssemblyVersion { get; set; }

        /// <summary>
        /// Informational version of this application
        /// </summary>
        public string AssemblyInformationalVersion { get; set; }

        /// <summary>
        /// Description of this application
        /// </summary>
        public string AssemblyDescription { get; set; }
    }
}
