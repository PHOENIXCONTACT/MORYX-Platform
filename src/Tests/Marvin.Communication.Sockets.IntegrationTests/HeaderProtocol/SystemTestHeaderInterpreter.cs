﻿namespace Marvin.Communication.Sockets.IntegrationTests
{
    public class SystemTestMessageInterpreter : HeaderMessageInterpreter<SystemTestHeader>
    {
        private static SystemTestMessageInterpreter _instance;
        public static SystemTestMessageInterpreter Instance => _instance ?? (_instance = new SystemTestMessageInterpreter());

        public SystemTestMessageInterpreter()
        {
            HeaderSize = new SystemTestHeader().ToBytes().Length;
        }

        /// <summary>
        /// Size of the header
        /// </summary>
        protected override int HeaderSize { get; }
    }
}