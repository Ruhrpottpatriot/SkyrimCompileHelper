// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationManagerViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   ViewModel containing methods and properties to manage configurations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    using Caliburn.Micro;

    using PropertyChanged;

    using SkyrimCompileHelper.Common;

    /// <summary>ViewModel containing methods and properties to manage configurations.</summary>
    [ImplementPropertyChanged]
    public sealed class ConfigurationManagerViewModel : Screen
    {
        /// <summary>The window manager.</summary>
        private readonly IWindowManager windowManager;

        /// <summary>Initializes a new instance of the <see cref="ConfigurationManagerViewModel"/> class.</summary>
        public ConfigurationManagerViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.DisplayName = "Edit Project Configurations";

                this.Configurations = new ObservableCollection<CompileConfiguration> 
                {
                    new CompileConfiguration { Name = "Name1" },
                    new CompileConfiguration { Name = "Name2" }
                };
            }
        }

        /// <summary>Initializes a new instance of the <see cref="ConfigurationManagerViewModel"/> class.</summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="configurations">The already existing configurations.</param>
        public ConfigurationManagerViewModel(IWindowManager windowManager, IList<CompileConfiguration> configurations)
        {
            this.DisplayName = "Edit Project Configurations";

            this.windowManager = windowManager;
            CompileConfiguration removeConfiguration = configurations.SingleOrDefault(c => c.Name == Constants.EditConst);
            this.Configurations = new ObservableCollection<CompileConfiguration>(configurations);
            this.Configurations.Remove(removeConfiguration);
        }

        /// <summary>Gets or sets the configurations.</summary>
        public ObservableCollection<CompileConfiguration> Configurations { get; set; }

        /// <summary>Gets or sets the selected configuration.</summary>
        public CompileConfiguration SelectedConfiguration { get; set; }
        
        /// <summary>Adds a new configuration.</summary>
        public void AddConfiguration()
        {
            AddConfigurationViewModel viewModel = new AddConfigurationViewModel();

            Dictionary<string, object> settingsDictionary = new Dictionary<string, object>
            {
                { "ResizeMode", ResizeMode.NoResize }
            };

            bool? answer = this.windowManager.ShowDialog(viewModel, null, settingsDictionary);

            if (answer.HasValue && answer.Value)
            {
                this.Configurations.Add(viewModel.GetConfiguration());
            }
        }

        /// <summary>Deletes a configuration.</summary>
        public void DeleteConfiguration()
        {
            this.Configurations.Remove(this.SelectedConfiguration);
        }

        /// <summary>Closes the screen.</summary>
        public void Close()
        {
            // Hack: Since we removed the edit item in the constructor, we need to re-add it here.
            this.Configurations.Add(new CompileConfiguration { Name = Constants.EditConst });
            this.TryClose(true);
        }
    }
}
