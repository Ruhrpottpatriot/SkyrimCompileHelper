// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Solution.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents a solution.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Common
{
    using System.Collections.Generic;

    using PropertyChanged;

    using Semver;

    /// <summary>Represents a solution.</summary>
    [ImplementPropertyChanged]
    public class Solution
    {
        /// <summary>Initialises a new instance of the <see cref="Solution"/> class.</summary>
        public Solution()
        {
            this.Version = new SemVersion(0);
        }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the compile configurations.</summary>
        public IList<CompileConfiguration> CompileConfigurations { get; set; }

        /// <summary>Gets or sets the selected configuration.</summary>
        public string SelectedConfiguration { get; set; }

        /// <summary>Gets or sets the version.</summary>
        public SemVersion Version { get; set; }

        /// <summary>Gets or sets the path.</summary>
        public string Path { get; set; }
    }
}