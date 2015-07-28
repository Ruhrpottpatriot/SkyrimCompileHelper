// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    using PropertyChanged;

    using Semver;

    using SkyrimCompileHelper.Common;

    /// <summary>ViewModel containing methods and properties to work with a solution.</summary>
    [ImplementPropertyChanged]
    public class SolutionViewModel : PropertyChangedBase
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>Initializes a new instance of the <see cref="SolutionViewModel"/> class.</summary>
        public SolutionViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.Name = "Outfits of Skyrim";
                this.Version = new SemVersion(0, 1);
                this.CompilerFlags = "Flags";
                this.Configurations = new List<CompileConfiguration>
                {
                    new CompileConfiguration { Name = "Debug" },
                    new CompileConfiguration { Name = "Release" },
                    new CompileConfiguration { Name = Constants.EditConst }
                };
            }
        }

        /// <summary>Initializes a new instance of the <see cref="SolutionViewModel"/> class.</summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="solution">The solution to work with.</param>
        public SolutionViewModel(IWindowManager windowManager, Solution solution)
        {
            this.windowManager = windowManager;
            this.Configurations = solution.CompileConfigurations ?? new List<CompileConfiguration>();
            this.Configurations.Add(new CompileConfiguration { Name = Constants.EditConst });
            this.Name = solution.Name;
            this.Version = solution.Version;
        }

        /// <summary>Gets or sets the solution name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the solution version.</summary>
        public SemVersion Version { get; set; }

        /// <summary>Gets or sets the compiler flags.</summary>
        public string CompilerFlags { get; set; }

        /// <summary>Gets or sets the compile configurations.</summary>
        public IList<CompileConfiguration> Configurations { get; set; }

        /// <summary>Opens the solution folder in the windows explorer.</summary>
        /// <exception cref="NotImplementedException">Not yet implemented</exception>
        public void OpenSolutionFolder()
        {
            throw new NotImplementedException();
        }

        /// <summary>Changes the version of a solution.</summary>
        public void ChangeVersion()
        {
            ChangeVersionViewModel viewModel = new ChangeVersionViewModel(this.Version);

            Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
            {
                { "ResizeMode", ResizeMode.NoResize } 
            };

            bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

            if (answer.HasValue && answer.Value)
            {
                this.Version = viewModel.GetVersion();
            }
        }

        /// <summary>Changes the compile configuration for the current solution.</summary>
        /// <param name="sender">The <see cref="ComboBox"/> that holds the selected item.</param>
        /// <exception cref="NotImplementedException">Thrown when the configuration manger is selected.</exception>
        public void ChangeConfiguration(ComboBox sender)
        {
            if (sender.SelectedItem == null)
            {
                return;
            }

            string configurationName = ((CompileConfiguration)sender.SelectedItem).Name;

            if (configurationName == Constants.EditConst)
            {
                ConfigurationManagerViewModel viewModel = new ConfigurationManagerViewModel(this.windowManager, this.Configurations);

                Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
                {
                    { "ResizeMode", ResizeMode.NoResize }
                };

                bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

                if (answer.HasValue && answer.Value)
                {
                    this.Configurations = viewModel.Configurations;
                }

                return;
            }

            CompileConfiguration configuration = this.Configurations.SingleOrDefault(c => c.Name == configurationName);
            this.CompilerFlags = configuration != null ? configuration.CompilerFlags : string.Empty;
        }

        /// <summary>Compiles the source files with the selected configuration.</summary>
        /// <exception cref="NotImplementedException">Not yet implemented.</exception>
        public void Compile()
        {
            throw new NotImplementedException();
        }
    }
}
