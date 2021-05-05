﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;
using Moryx.Runtime.Configuration;
using Moryx.Runtime.Container;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.Threading;

namespace Moryx.Runtime.Kestrel.Maintenance
{
    /// <summary>
    /// Controller logger
    /// </summary>
    [ApiController]
    [Route("modules")]
    [ServiceName("IModuleMaintenance")]
    internal class ModulesController : Controller
    {
        #region dependency injection

        public IModuleManager ModuleManager { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        public IRuntimeConfigManager ConfigManager { get; set; }

        [UseChild("ModuleMaintenance")]
        public IModuleLogger Logger { get; set; }

        #endregion

        /// <summary>
        /// Get all server modules.
        /// </summary>
        /// <returns>A list of the server modules.</returns>
        [HttpGet]
        [Produces("application/json")]
        public ServerModuleModel[] GetAll()
        {
            var models = new List<ServerModuleModel>(ModuleManager.AllModules.Count());
            foreach (var module in ModuleManager.AllModules)
            {
                var notifications = module.Notifications.ToArray();

                var model = new ServerModuleModel
                {
                    Name = module.Name,
                    Assembly = ConvertAssembly(module),
                    HealthState = module.State,
                    StartBehaviour = ModuleManager.BehaviourAccess<ModuleStartBehaviour>(module).Behaviour,
                    FailureBehaviour = ModuleManager.BehaviourAccess<FailureBehaviour>(module).Behaviour,
                    Notifications = notifications.Select(n => new NotificationModel(n)).ToArray()
                };

                var dependencies = ModuleManager.StartDependencies(module);
                foreach (var dependency in dependencies)
                {
                    model.Dependencies.Add(new ServerModuleModel
                    {
                        Name = dependency.Name,
                        HealthState = dependency.State
                    });
                }
                models.Add(model);
            }
            return models.ToArray();
        }

        /// <summary>
        /// Get health state
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        [HttpGet("module/{moduleName}/healthstate")]
        [Produces("application/json")]
        public ActionResult<ServerModuleState> HealthState(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
            {
                return Ok(module.State);
            }

            return NotFound(ServerModuleState.Failure);
        }

        /// <summary>
        /// Get health state
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        [HttpGet("module/{moduleName}/notifications")]
        [Produces("application/json")]
        public ActionResult<NotificationModel[]> Notifications(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
            {
                return Ok(module.Notifications.Select(n => new NotificationModel(n)).ToArray());
            }

            return NotFound(new NotificationModel[0]);
        }

        /// <summary>
        /// Gets the dependency evaluation.
        /// </summary>
        /// <returns>The dependency evaluation.</returns>
        [HttpGet("dependencies")]
        [Produces("application/json")]
        public DependencyEvaluation GetDependencyEvaluation()
        {
            return new DependencyEvaluation(ModuleManager.DependencyTree);
        }

        /// <summary>
        /// Try to start the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be started.</param>
        [HttpPost("module/{moduleName}/start")]
        public void Start(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ModuleManager.StartModule(module);
        }

        /// <summary>
        /// Try to stop the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be stopped.</param>
        [HttpPost("module/{moduleName}/stop")]
        public void Stop(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ModuleManager.StopModule(module);
        }

        /// <summary>
        /// Try to reincarnate the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be reincarnated.</param>
        [HttpPost("module/{moduleName}/reincarnate")]
        public void Reincarnate(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            ParallelOperations.ExecuteParallel(ModuleManager.ReincarnateModule, module);
        }

        /// <summary>
        /// Update the modules failure and start behavior
        /// </summary>
        [HttpGet("module/{moduleName}")]
        [Consumes("application/json")]
        public void Update(string moduleName, [FromBody] ServerModuleModel module)
        {
            var serverModule = GetModuleFromManager(moduleName);

            var startBehaviour = ModuleManager.BehaviourAccess<ModuleStartBehaviour>(serverModule);
            if (startBehaviour.Behaviour != module.StartBehaviour)
            {
                Logger.Log(LogLevel.Info, "Changing start behaviour of {0} to {1}", moduleName, module.StartBehaviour);
                startBehaviour.Behaviour = module.StartBehaviour;
            }

            var failureBehaviour = ModuleManager.BehaviourAccess<FailureBehaviour>(serverModule);
            if (failureBehaviour.Behaviour != module.FailureBehaviour)
            {
                Logger.Log(LogLevel.Info, "Changing failure behaviour of {0} to {1}", moduleName, module.FailureBehaviour);
                failureBehaviour.Behaviour = module.FailureBehaviour;
            }
        }

        /// <summary>
        /// Confirms the warning of the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module where the warning will confirmed.</param>
        [HttpPost("module/{moduleName}/confirm")]
        public void ConfirmWarning(string moduleName)
        {
            var module = GetModuleFromManager(moduleName);
            foreach (var notification in module.Notifications.ToArray())
            {
                notification.Confirm();
            }
            ModuleManager.InitializeModule(module);
        }

        /// <summary>
        /// Get the config for the module from the moduleName.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <returns>Configuration of the requested module.</returns>
        [HttpGet("module/{moduleName}/config")]
        [Produces("application/json")]
        public ActionResult<Config> GetConfig(string moduleName)
        {
            Logger.Log(LogLevel.Info, "Converting config of plugin {0}", moduleName);
            try
            {
                var module = GetModuleFromManager(moduleName);
                var serialization = CreateSerialization(module);

                var config = GetConfig(module, false);
                var configModel = new Config
                {
                    Root = EntryConvert.EncodeObject(config, serialization)
                };

                return Ok(configModel);
            }
            catch (Exception ex)
            {
                return new ObjectResult($"{ex.GetType().Name}:{ex.Message}") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// Set the given config and react to updated mode.
        /// </summary>
        [HttpPost("module/{moduleName}/config")]
        [Consumes("application/json")]
        public IActionResult SetConfig(string moduleName, [FromBody] SaveConfigRequest request)
        {
            try
            {
                var module = GetModuleFromManager(moduleName);
                var serialization = CreateSerialization(module);
                var config = GetConfig(module, true);
                EntryConvert.UpdateInstance(config, request.Config.Root, serialization);
                ConfigManager.SaveConfiguration(config, request.UpdateMode == ConfigUpdateMode.UpdateLiveAndSave);

                if (request.UpdateMode == ConfigUpdateMode.SaveAndReincarnate)
                    // This has to be done parallel so we can also reincarnate the Maintenance itself
                    ParallelOperations.ExecuteParallel(() => ModuleManager.ReincarnateModule(module));

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Warning, ex, "Failed to save config of {0}", moduleName);
                return new ObjectResult($"{ex.GetType().Name}:{ex.Message}") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// Get all server modules.
        /// </summary>
        /// <returns>A list of the server modules.</returns>
        [HttpGet("module/{moduleName}/console")]
        [Produces("application/json")]
        public MethodEntry[] GetMethods(string moduleName)
        {
            var methods = new MethodEntry[] { };
            var serverModule = GetModuleFromManager(moduleName);

            if (serverModule?.Console != null)
            {
                methods = EntryConvert.EncodeMethods(serverModule.Console, CreateEditorSerializeSerialization(serverModule)).ToArray();
            }

            return methods;
        }

        /// <summary>
        /// Invokes a method
        /// </summary>
        [HttpPost("module/{moduleName}/console")]
        [Produces("application/json")]
        public ActionResult<Entry> InvokeMethod(string moduleName, [FromBody] MethodEntry method)
        {
            Entry result = null;
            var serverModule = GetModuleFromManager(moduleName);

            if (serverModule != null && method != null)
            {
                try
                {
                    result = EntryConvert.InvokeMethod(serverModule.Console, method,
                        CreateEditorSerializeSerialization(serverModule));
                }
                catch (Exception e)
                {
                    result = new Entry
                    {
                        Description = $"Error while invoking function: {method.DisplayName}",
                        DisplayName = "Error description",
                        Identifier = "0",
                        Value = new EntryValue { Current = e.Message, Type = EntryValueType.String }
                    };
                }
            }
            else
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Create serialization for this module
        /// </summary>
        private ICustomSerialization CreateSerialization(IModule module)
        {
            var host = (IContainerHost)module;
            return new PossibleValuesSerialization(host.Container, ConfigManager)
            {
                FormatProvider = Thread.CurrentThread.CurrentUICulture
            };
        }

        /// <summary>
        /// Create serialization for this module
        /// </summary>
        private ICustomSerialization CreateEditorSerializeSerialization(IModule module)
        {
            var host = (IContainerHost)module;
            return new AdvancedEntrySerializeSerialization(host.Container, ConfigManager)
            {
                FormatProvider = Thread.CurrentThread.CurrentUICulture
            };
        }

        /// <summary>
        /// Get the config type
        /// </summary>
        /// <returns></returns>
        private IConfig GetConfig(IModule module, bool copy)
        {
            var moduleType = module.GetType();
            var configType = moduleType.BaseType != null && moduleType.BaseType.IsGenericType
                ? moduleType.BaseType.GetGenericArguments()[0]
                : moduleType.Assembly.GetTypes().FirstOrDefault(type => typeof(IConfig).IsAssignableFrom(type));

            return ConfigManager.GetConfiguration(configType, copy);
        }

        private IServerModule GetModuleFromManager(string moduleName)
        {
            var module = ModuleManager.AllModules.FirstOrDefault(m => m.Name == moduleName);
            if (module == null)
                throw new ArgumentException("Found no module with the given name!", moduleName);
            return module;
        }

        private static AssemblyModel ConvertAssembly(IInitializable service)
        {
            var assembly = service.GetType().Assembly;
            var assemblyName = assembly.GetName();

            var assemblyVersion = assemblyName.Version;
            var fileVersionAttr = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var informationalVersionAttr = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            var model = new AssemblyModel
            {
                Name = assemblyName.Name + ".dll",
                Version = assemblyVersion.Major + "." + assemblyVersion.Minor + "." + assemblyVersion.Build,
                FileVersion = fileVersionAttr?.Version,
                InformationalVersion = informationalVersionAttr?.InformationalVersion
            };
            return model;
        }
    }
}
