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
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    using Caliburn.Micro;

    using SkyrimCompileHelper.Repositories;
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
    }
}
