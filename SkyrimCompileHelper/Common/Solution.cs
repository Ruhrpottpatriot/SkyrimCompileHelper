﻿namespace SkyrimCompileHelper.Common
{
    using System.Collections.Generic;

    using PropertyChanged;

    using Semver;

    [ImplementPropertyChanged]
    public class Solution
    {
        /// <summary>Initializes a new instance of the <see cref="Solution"/> class.</summary>
        public Solution()
        {
            this.Version = new SemVersion(0);
        }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the compile configurations.</summary>
        public IList<CompileConfiguration> CompileConfigurations { get; set; }

        /// <summary>Gets or sets the selected configuration.</summary>
        public CompileConfiguration SelectedConfiguration { get; set; }

        /// <summary>Gets or sets the version.</summary>
        public SemVersion Version { get; set; }

        /// <summary>Gets or sets the path.</summary>
        public string Path { get; set; }
    }
}