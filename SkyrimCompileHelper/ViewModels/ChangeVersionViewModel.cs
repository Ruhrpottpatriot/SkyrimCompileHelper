// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeVersionViewModel.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   ViewModel containing methods and properties to change the version.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.ViewModels
{
    using Caliburn.Micro;

    using PropertyChanged;

    using Semver;

    /// <summary>ViewModel containing methods and properties to change the version.</summary>
    [ImplementPropertyChanged]
    public sealed class ChangeVersionViewModel : Screen
    {
        /// <summary>Initialises a new instance of the <see cref="ChangeVersionViewModel"/> class.</summary>
        public ChangeVersionViewModel()
        {
            if (Execute.InDesignMode)
            {
                this.Major = 0;
                this.Minor = 1;
                this.Patch = 0;
                this.Prerelease = "alpha";
                this.Build = "1";
            }

            this.DisplayName = "Change Version";
        }

        /// <summary>Initialises a new instance of the <see cref="ChangeVersionViewModel"/> class.</summary>
        /// <param name="version">The version that is going to be changed.</param>
        public ChangeVersionViewModel(SemVersion version)
        {
            this.Major = version.Major;
            this.Minor = version.Minor;
            this.Patch = version.Patch;
            this.Prerelease = version.Prerelease;
            this.Build = version.Build;
        }

        /// <summary>Gets or sets the major version component.</summary>
        public int Major { get; set; }

        /// <summary>Gets or sets the minor version component.</summary>
        public int Minor { get; set; }

        /// <summary>Gets or sets the patch version component.</summary>
        public int Patch { get; set; }

        /// <summary>Gets or sets the prerelease version component.</summary>
        public string Prerelease { get; set; }

        /// <summary>Gets or sets the build version component.</summary>
        public string Build { get; set; }

        /// <summary>Compiles and retrieves the version from the ViewModel.</summary>
        /// <returns>The compiled <see cref="SemVersion"/>.</returns>
        public SemVersion GetVersion()
        {
            return new SemVersion(this.Major, this.Minor, this.Patch, this.Prerelease, this.Build);
        }

        /// <summary>Closes the windows with a success code.</summary>
        public void Close()
        {
            this.TryClose(true);
        }

        /// <summary>Closes the window with an error code.</summary>
        public void Save()
        {
            this.TryClose(false);
        }
    }
}
