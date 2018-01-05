﻿using System;
using System.Collections.Generic;
using System.Threading;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Runtime.Container;
using Marvin.Runtime.Wcf;
using Marvin.StateMachines;
using Marvin.Threading;
using Marvin.Tools;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Base class for a server module. Provides all necessary methods to be a server module.  
    /// </summary>
    /// <typeparam name="TConf">Configuration type for the server module.</typeparam>
    public abstract class ServerModuleBase<TConf> : IServerModule, IContainerHost, IServerModuleStateContext, ILoggingHost, ILoggingComponent
        where TConf : class, IConfig, new()
    {
        /// <summary>
        /// Unique name for a module within the platform it is designed for
        /// </summary>
        public abstract string Name { get; }

        IServerModuleConsole IServerModule.Console => Container?.Resolve<IServerModuleConsole>();

        /// <summary>
        /// Notifications raised within module and during state changes.
        /// </summary>
        public INotificationCollection Notifications { get; } = new ServerNotificationCollection();

        /// <summary>
        /// Creates a new instance of <see cref="ServerModuleBase{TConf}"/> and initializes the state machine
        /// </summary>
        protected ServerModuleBase()
        {
            StateMachine.Initialize((IServerModuleStateContext)this).With<ServerModuleStateBase>();
        }

        #region ValidateHealthState

        /// <summary>
        /// Does a validaton of the health state. Only states indicating <see cref="ServerModuleState.Running"/> will not 
        /// throw an <see cref="HealthStateException"/>
        /// </summary>
        protected void ValidateHealthState() => _state.ValidateHealthState();

        /// <inheritdoc />
        void IServerModuleStateContext.InvalidHealthState(ServerModuleState state)
        {
            throw new HealthStateException(state);
        }

        #endregion

        #region Logging

        /// <summary>
        /// <see cref="ILoggerManagement"/> 
        /// </summary>
        public ILoggerManagement LoggerManagement { get; set; }

        /// <summary>
        /// Logger of the current state.
        /// </summary>
        public IModuleLogger Logger { get; set; }

        #endregion

        #region Server Module methods

        /// <summary>
        /// <see cref="IModuleContainerFactory"/>
        /// </summary>
        public IModuleContainerFactory ContainerFactory { get; set; }

        void IInitializable.Initialize()
        {
            _state.Initialize();
        }

        void IServerModuleStateContext.Initialize()
        {
            // Activate logging
            LoggerManagement.ActivateLogging(this);
            Logger.LogEntry(LogLevel.Info, "{0} is initializing...", Name);
            
            // Get config and parse for container settings
            Config = ConfigManager.GetConfiguration<TConf>();
            ConfigParser.ParseStrategies(Config, Strategies);

            // Initizalize container with server module dll and this dll
            Container = ContainerFactory.Create(Strategies, GetType().Assembly)
                .SetInstance<IModuleErrorReporting>(this)
                .Register<IParallelOperations, ParallelOperations>()
                // Register instances for this cycle
                .SetInstance(Config).SetInstance(Logger);

            // Activate WCF
            EnableWcf(HostFactory);

            OnInitialize();

            var subInits = Container.ResolveAll<ISubInitializer>() ?? new ISubInitializer[0];
            foreach (var subInitializer in subInits)
            {
                subInitializer.Initialize(Container);
            }

            Logger.LogEntry(LogLevel.Info, "{0} initialized!", Name);

            // After initializing the module, all notifications are unnecessary
            Notifications.Clear();
        }

        void IServerModule.Start()
        {
            _state.Start();
        }

        void IServerModuleStateContext.Start()
        {
            Logger.LogEntry(LogLevel.Info, "{0} is starting...", Name);

            OnStart();

            Logger.LogEntry(LogLevel.Info, "{0} started!", Name);
        }

        void IServerModule.Stop()
        {
            _state.Stop();
        }

        void IServerModuleStateContext.Stop()
        {
            Logger.LogEntry(LogLevel.Info, "{0} is stopping...", Name);

            try
            {
                OnStop();
            }
            finally
            {
                // Destroy local container
                if (Container != null)
                {
                    Container.Destroy();
                    Container = null;
                }
                // Deregister from logging
                Logger.LogEntry(LogLevel.Info, "{0} stopped!", Name);
                LoggerManagement.DeactivateLogging(this);
            }
        }

        #endregion

        #region Container

        /// <summary>
        /// Internal container can only be set from inside this assembly
        /// </summary>
        public IContainer Container { get; private set; }

        /// <summary>
        /// Configuration used for the container
        /// </summary>
        public IDictionary<Type, string> Strategies { get; } = new Dictionary<Type, string>();

        /// <summary>
        /// Wcf host factory to open wcf services
        /// </summary>
        public IWcfHostFactory HostFactory { get; set; }

        /// <summary>
        /// Enable wcf for this module
        /// </summary>
        private void EnableWcf(IWcfHostFactory hostFactory)
        {
            var typedFactory = Container.Resolve<ITypedHostFactory>();
            Container.SetInstance((IConfiguredHostFactory)new ConfiguredHostFactory(hostFactory)
            {
                Factory = typedFactory,
                Logger = Logger
            });
        }

        #endregion

        #region Child module transitions

        /// <summary>
        /// Code executed on start up and after service was stopped and should be started again.
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// Code executed after OnInitialize
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Code executed when service is stopped
        /// </summary>
        protected abstract void OnStop();

        #endregion

        #region Configuration

        /// <summary>
        /// Config manager kernel component used to access this module config
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Config instance for the current lifecycle
        /// </summary>
        protected TConf Config { get; private set; }

        #endregion

        #region IServerModuleState

        private ServerModuleStateBase _state;

        void IStateContext.SetState(IState state)
        {
            var oldState = ServerModuleState.Stopped;
            if (_state != null)
                oldState = _state.Classification;

            _state = (ServerModuleStateBase)state;

            StateChange(oldState, State);
        }

        /// <summary>
        /// The current state.
        /// </summary>
        public ServerModuleState State => _state.Classification;

        /// <summary>
        /// Event is called when ModuleState changed
        /// </summary>
        public event EventHandler<ModuleStateChangedEventArgs> StateChanged;
        private void StateChange(ServerModuleState oldState, ServerModuleState newState)
        {
            if (StateChanged == null || oldState == newState)
            {
                return;
            }

            // Since event handling may take a while make sure we don't stop module execution
            foreach (var caller in StateChanged.GetInvocationList())
            {
                ThreadPool.QueueUserWorkItem(delegate (object callObj)
                {
                    try
                    {
                        var callDelegate = (Delegate)callObj;
                        callDelegate.DynamicInvoke(this, new ModuleStateChangedEventArgs { OldState = oldState, NewState = newState });
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogException(LogLevel.Warning, ex, "Failed to notify listener of state change");
                    }
                }, caller);
            }
        }

        #endregion

        #region ErrorReporting

        /// <summary>
        /// Report internal failure to parent module
        /// </summary>
        /// <param name="sender">The sender of the failure report.</param>
        /// <param name="exception">The exception which should be reported.</param>
        void IModuleErrorReporting.ReportFailure(object sender, Exception exception)
        {
            var notification = new FailureNotification(exception, $"Component {sender.GetType().Name} reported an exception");
            LogNotification(sender, notification);

            _state.ErrorOccured();
        }

        /// <summary>
        /// Report an error to be treated as a warning
        /// </summary>
        /// <param name="sender">The sender of the warning report.</param>
        /// <param name="exception">The exception which should be reported as warning.</param>
        void IModuleErrorReporting.ReportWarning(object sender, Exception exception)
        {
            var notification = new WarningNotification(n => Notifications.Remove(n), exception,
                $"Component {sender.GetType().Name} reported an exception");
            LogNotification(sender, notification);
        }

        /// <inheritdoc />
        void IServerModuleStateContext.LogNotification(object sender, IModuleNotification notification)
        {
            LogNotification(sender, notification);
        }

        private void LogNotification(object sender, IModuleNotification notification)
        {
            Notifications.Add(notification);

            var loggingComponent = sender as ILoggingComponent;
            var logger = loggingComponent != null ? loggingComponent.Logger : Logger;
            logger.LogException(notification.Type == NotificationType.Warning ? LogLevel.Warning : LogLevel.Error,
                notification.Exception, notification.Message);
        }

        #endregion

        /// <summary>
        /// Override to provide specific information about the server module.
        /// </summary>
        /// <returns>Name and current health state of the module.d</returns>
        public override string ToString()
        {
            return $"{Name} - {State}";
        }
    }
}