// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Provides methods and properties to edit <see cref="CompileConfiguration" /> details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using System.Threading.Tasks;
    using Caliburn.Micro;

    using PapyrusCompiler;

    using PropertyChanged;

    using SkyrimCompileHelper.Core;
    using SkyrimCompileHelper.Core.EventHandles;

    /// <summary>Provides methods and properties to edit <see cref="CompileConfiguration"/> details.</summary>
    [ImplementPropertyChanged]
    public class ConfigurationViewModel
    {
        /// <summary>The event aggregator instance.</summary>
        private readonly IEventAggregator eventAggregator;

        /// <summary>Initialises a new instance of the <see cref="ConfigurationViewModel"/> class.</summary>
        public ConfigurationViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.FlagsFile = "TESV_Flags.flg";
                this.All = true;
                this.Optimize = true;
            }
        }

        /// <summary>Initialises a new instance of the <see cref="ConfigurationViewModel"/> class.</summary>
        /// <param name="eventAggregator">An event aggregator instance used to publish application wide messages.</param>
        /// <param name="configuration">The configuration to be used with this ViewModel.</param>
        public ConfigurationViewModel(IEventAggregator eventAggregator, CompileConfiguration configuration)
        {
            this.eventAggregator = eventAggregator;

            // Set sensible defaults, if the configuration is null
            if (configuration == null)
            {
                this.All = true;
                this.Debug = false;
                this.Optimize = false;
                this.Quiet = true;
                this.FlagsFile = "TESV_Papyrus_Flags.flg";
                this.SelectedAssemblyOption = AssemblyOption.AssembleAndDelete;
                
                // Since we want to keep the defaults, we return here.
                return;
            }

            this.All = configuration.All;
            this.Debug = configuration.Debug;
            this.Optimize = configuration.Optimize;
            this.Quiet = configuration.Quiet;
            this.FlagsFile = configuration.FlagFile;
            this.SelectedAssemblyOption = configuration.AssemblyOption;
        }

        /// <summary>Gets or sets the compiler flags.</summary>
        public string FlagsFile { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should compile a whole directory.</summary>
        public bool All { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should supress output.</summary>
        public bool Quiet { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should print debug information.</summary>
        public bool Debug { get; set; }

        /// <summary>Gets or sets a value indicating whether the source files should be copied after a compile.</summary>
        public bool CopySourceFiles { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should optimize the script files.</summary>
        public bool Optimize { get; set; }

        /// <summary>Gets or sets the selected assembly option.</summary>
        public AssemblyOption SelectedAssemblyOption { get; set; }

        /// <summary>Publishes a message on the <see cref="IEventAggregator"/> to notify the application to save the solution.</summary>
        public void SaveSolution()
        {
            this.eventAggregator.Publish(new SaveSolutionEvenHandle(), action => { Task.Factory.StartNew(action); });
        }
    }
}
