// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Default implementation of a Command handler. Handles all commands which can not be handled specific.
    /// </summary>
    internal class DefaultHandler : ICommandHandler
    {
        /// <summary>
        /// Check if this command can be handled.
        /// </summary>
        public bool CanHandle(string command)
        {
            return true;
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            Console.WriteLine("Command \"{0}\" unknown!", string.Join(" ", fullCommand));
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
        }
    }
}
