// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefBootstrapper.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   The bootstrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    using Caliburn.Micro;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
    using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

    using SkyrimCompileHelper.Common;
    using SkyrimCompileHelper.ViewModels;

    /// <summary>The bootstrapper.</summary>
    public sealed class MefBootstrapper : BootstrapperBase
    {
        /// <summary>The container.</summary>
        private CompositionContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MefBootstrapper"/> class.
        /// </summary>
        public MefBootstrapper()
        {
            this.Initialize();
        }

        /// <summary>
        /// Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param><param name="e">The args.</param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            this.DisplayRootViewFor<ShellViewModel>();
        }

        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            // Add the assembly source to the catalog.
            // ReSharper disable once RedundantEnumerableCastCall
            var catalog = new AggregateCatalog(AssemblySource.Instance.Select(i => new AssemblyCatalog(i)).OfType<ComposablePartCatalog>());

            // Create a new composition container.
            // ReSharper disable once RedundantEnumerableCastCall
            this.container = new CompositionContainer();

            // Create a new composition container.
            this.container = new CompositionContainer(catalog);

            CompositionBatch compositionBatch = new CompositionBatch();

            // Add EventAggregator to composition batch.
            compositionBatch.AddExportedValue<IEventAggregator>(new EventAggregator());
            compositionBatch.AddExportedValue<IWindowManager>(new WindowManager());
            compositionBatch.AddExportedValue<ISettingsRepository>(new SettingsRepository());
            compositionBatch.AddExportedValue<ISolutionRepository>(new SolutionRepository());
            compositionBatch.AddExportedValue(new LogWriter(this.BuildLoggingConfiguration()));

            // Add the container itself.
            compositionBatch.AddExportedValue(this.container);

            // Compose the container.
            this.container.Compose(compositionBatch);
        }

        /// <summary>Override this to provide an IoC specific implementation.</summary>
        /// <param name="service">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        protected override object GetInstance(Type service, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(service) : key;
            List<object> exports = this.container.GetExportedValues<object>(contract).ToList();

            if (exports.Any())
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        /// <summary>Override this to provide an IoC specific implementation</summary>
        /// <param name="serviceType">The service to locate.</param> 
        /// <returns>The located services.</returns>
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return this.container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        /// <summary>Override this to provide an IoC specific implementation.</summary>
        /// <param name="instance"> The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            this.container.SatisfyImportsOnce(instance);
        }

        /// <summary>Override to tell the framework where to find assemblies to inspect for views, etc.</summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] 
            {
                Assembly.GetExecutingAssembly()
            };
        }

        /// <summary>Builds the configuration used to log entries to the file system.</summary>
        /// <returns>A <see cref="LoggingConfiguration"/> with default settings.</returns>
        private LoggingConfiguration BuildLoggingConfiguration()
        {
            TextFormatter formatter = new TextFormatter("Timestamp: {timestamp(local)}{newline}Message: {message}{newline}Category: {category}{newline}Priority: {priority}{newline}EventId: {eventid}{newline}ActivityId: {property(ActivityId)}{newline}Severity: {severity}{newline}Title:{title}{newline}");

            ICollection<string> categories = new List<string> { "BlockedByFilter" };

            PriorityFilter priorityFilter = new PriorityFilter("PriorityFilter", -1);
            LogEnabledFilter logEnabledFilter = new LogEnabledFilter("LogEnabled Filter", true);
            CategoryFilter categoryFilter = new CategoryFilter("CategoryFilter", categories, CategoryFilterMode.AllowAllExceptDenied);

            RollingFlatFileTraceListener rollingFileListener = new RollingFlatFileTraceListener(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SHC\Logs\Everything.log"),
                "----------------------------------------",
                "----------------------------------------",
                formatter,
                20,
                "yyyy-MM-dd",
                RollFileExistsBehavior.Increment,
                RollInterval.None,
                5);

            RollingFlatFileTraceListener errorFileListener = new RollingFlatFileTraceListener(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SHC\Logs\Errors.log"),
                "----------------------------------------",
                "----------------------------------------",
                formatter,
                20,
                "yyyy-MM-dd",
                RollFileExistsBehavior.Increment,
                RollInterval.None,
                2);

            // Build Configuration
            LoggingConfiguration config = new LoggingConfiguration();
            config.Filters.Add(priorityFilter);
            config.Filters.Add(logEnabledFilter);
            config.Filters.Add(categoryFilter);

            config.AddLogSource("General", SourceLevels.All, true, rollingFileListener);
            config.AddLogSource("Compiler", SourceLevels.All, true, rollingFileListener);
            config.AddLogSource("Error", SourceLevels.Error, true, errorFileListener);

            return config;
        }
    }
}
