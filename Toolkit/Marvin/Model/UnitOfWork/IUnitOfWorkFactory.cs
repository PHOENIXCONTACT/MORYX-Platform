//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Entity Developer tool using the Marvin template for generating Repositories and a Unit of Work for Entity Framework model.
// If you have any questions or suggestions for improvement regarding this code, contact Thomas Fuchs.
// Code is generated on: 25.09.2013 15:01:35
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System;
using Marvin.Container;

namespace Marvin.Model
{
    /// <summary>
    /// Factory to open db context wrapped in UnitOfWork
    /// </summary>
    public interface IUnitOfWorkFactory : INamedChildContainer<IUnitOfWorkFactory>
    {
        /// <summary>
        /// Create new context in default mode ProxyLazyLoadingTracking
        /// </summary>
        /// <returns></returns>
        IUnitOfWork Create();

        /// <summary>
        /// Specify the context mode
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        IUnitOfWork Create(ContextMode mode);

        /// <summary>
        /// Tries to merge a new context of this model
        /// with an open one of another model. Also used
        /// to load an inherited model using an existing
        /// parent context.
        /// </summary>
        IUnitOfWork Merge(IUnitOfWork other);

        /// <summary>
        /// Tries to merge a new context of this model
        /// with an open one of another model. Also used
        /// to load an inherited model using an existing
        /// parent context. The given context allows to
        /// modify the behaviour of the new and underlying
        /// context
        /// </summary>
        IUnitOfWork Merge(IUnitOfWork other, ContextMode mode);

        /// <summary>
        /// Read database config
        /// </summary>
        void ReloadConfiguration();
    }

    /// <summary>
    /// Configuration for the context creation. It will alter the values for entity tracking, proxy creation
    /// and lazy loading. Each mode has its advantages and disadvantages
    /// Flags: Tracking LazyLoading Proxy
    /// </summary>
    [Flags]
    public enum ContextMode
    {
        /// <summary>
        /// Don't use any context enhancements - simple database access. This mode is used best for read-only
        /// operations and LinQ queries. This will not track changes made to entities and does not provide 
        /// deferred loading of entities.
        /// </summary>
        AllOff = 0x0,

        /// <summary>
        /// Wrap each requested entity into a proxy class for enhanced EF features like lazy loading. Using this
        /// stand alone does not provide any benefit and should only be done as an intermediate step for performance
        /// enhancement.
        /// </summary>
        Proxy = 0x1,

        /// <summary>
        /// Keep track of entities and changes made to them. This works with and without proxies. Tracking is also
        /// required for deferred loading of navigation properties. This mode is ideal for reading operations split
        /// into several minor reads. Start with loading the root entity and load navigation properties on-demand
        /// using the repository Load-method.
        /// </summary>
        Tracking = 0x4,

        /// <summary>
        /// Use proxies and track all changes made to them
        /// </summary>
        ProxyTracking = 0x5,
        /// <summary>
        /// Use the full available feature set of proxies, lazy loading and tracking. This is the default mode and also
        /// the easiest. Used best for duplex database access involving read, insert and update.
        /// </summary>
        AllOn = 0x7,
    }
}