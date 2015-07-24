namespace SkyrimCompileHelper.Common
{
    using System.Collections.Generic;

    using PropertyChanged;

    using Semver;

    [ImplementPropertyChanged]
    public class ModRepository
    {
        /// <summary>The version.</summary>
        private SemVersion version;

        /// <summary>Initializes a new instance of the <see cref="ModRepository"/> class.</summary>
        public ModRepository()
        {
            this.version = new SemVersion(0);
        }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the compile configurations.</summary>
        public IEnumerable<CompileConfiguration> CompileConfigurations { get; set; }

        /// <summary>Gets or sets the selected configuration.</summary>
        public CompileConfiguration SelectedConfiguration { get; set; }

        /// <summary>Gets or sets the version.</summary>
        public string Version
        {
            get
            {
                return this.version.ToString();
            }

            set
            {
                this.version = SemVersion.Parse(value);
            }
        }
        
        /// <summary>Gets or sets the path.</summary>
        public string Path { get; set; }
    }
}