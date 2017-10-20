﻿using System;
using Marvin.Modules;

namespace Marvin.Model
{
    /// <summary>
    /// Registration attribute for data model factories
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ModelFactoryAttribute : ModelAttribute
    {
        /// <summary>
        /// Register this factory using the models namespace
        /// </summary>
        /// <param name="modelNamespace">Namespace of the model</param>
        public ModelFactoryAttribute(string modelNamespace)
            : base(typeof(IUnitOfWorkFactory), typeof(IInitializable))
        {
            Name = modelNamespace;
        }
    }
}