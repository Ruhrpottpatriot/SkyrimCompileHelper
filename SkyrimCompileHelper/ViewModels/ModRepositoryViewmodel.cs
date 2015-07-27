// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModRepositoryViewmodel.cs" company="Robert Logiewa">
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

    /// <summary>ViewModel containing methods and properties to work with a mod repository.</summary>
    [ImplementPropertyChanged]
    public class ModRepositoryViewModel : PropertyChangedBase
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>Initializes a new instance of the <see cref="ModRepositoryViewModel"/> class.</summary>
        public ModRepositoryViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.Name = "Outfits of Skyrim";
                this.Version = new SemVersion(0, 1);
                this.CompilerFlags = "Flags";
                // this.Configurations = new List<string> { "Debug", "Release", ConfigurationManger };
            }
        }

        /// <summary>Initializes a new instance of the <see cref="ModRepositoryViewModel"/> class.</summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="repository">The mod repository to work with.</param>
        public ModRepositoryViewModel(IWindowManager windowManager, ModRepository repository)
        {
            this.windowManager = windowManager;
            this.Configurations = repository.CompileConfigurations ?? new List<CompileConfiguration>();
            this.Configurations.Add(new CompileConfiguration { Name = Constants.EditConst });
            this.Name = repository.Name;
            this.Version = repository.Version;
        }

        /// <summary>Gets or sets the repository name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the repository version.</summary>
        public SemVersion Version { get; set; }

        /// <summary>Gets or sets the compiler flags.</summary>
        public string CompilerFlags { get; set; }

        /// <summary>Gets or sets the compile configurations.</summary>
        public IList<CompileConfiguration> Configurations { get; set; }

        /// <summary>Changes the version of a mod repository.</summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
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
                this.Version = viewModel.GetVersion().ToString();
            }
        }

        /// <summary>Changes the compile configuration for the current repository.</summary>
        /// <param name="sender">The <see cref="ComboBox"/> that holds the selected item.</param>
        /// <exception cref="NotImplementedException">Thrown when the configuration manger is selected.</exception>
        public void ChangeConfiguration(ComboBox sender)
        {
            string value = ((CompileConfiguration)sender.SelectedItem).Name;

            if (value == Constants.EditConst)
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

            CompileConfiguration configuration = this.Configurations.SingleOrDefault(c => c.Name == value);
            this.CompilerFlags = configuration != null ? configuration.CompilerFlags : string.Empty;
        }

        public void Compile()
        {
            throw new NotImplementedException();
        }
    }
}
