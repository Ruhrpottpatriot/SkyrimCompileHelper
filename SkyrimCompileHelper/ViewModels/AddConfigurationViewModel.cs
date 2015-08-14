// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddConfigurationViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   ViewModel containing methods and properties to create new configurations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using Caliburn.Micro;

    using SkyrimCompileHelper.Common;

    /// <summary>ViewModel containing methods and properties to create new configurations.</summary>
    public class AddConfigurationViewModel : Screen
    {
        /// <summary>Initialises a new instance of the <see cref="AddConfigurationViewModel"/> class.</summary>
        public AddConfigurationViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.ConfigurationName = "Test Configuration Name";
            }
        }

        /// <summary>Gets or sets the configuration name.</summary>
        public string ConfigurationName { get; set; }

        /// <summary>Closes the window saving all changes.</summary>
        public void Save()
        {
            this.TryClose(true);
        }

        /// <summary>Closes the window discarding all changes.</summary>
        public void Cancel()
        {
            this.TryClose(false);
        }

        /// <summary>Compiles the configuration for further use.</summary>
        /// <returns>The compiled <see cref="CompileConfiguration"/>.</returns>
        public CompileConfiguration GetConfiguration()
        {
            return new CompileConfiguration
            {
                Name = this.ConfigurationName,
                FlagFile = "TESV_Papyrus_Flags.flg",
                All = true,
                Quiet = false,
                Debug = true,
                Optimize = false
            };
        }
    }
}
