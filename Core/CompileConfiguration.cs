// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompileConfiguration.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents a cimpoler configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Core
{
    using System;
    using System.Collections.Generic;

    using PapyrusCompiler;

    /// <summary>Represents a compiler configuration.</summary>
    public class CompileConfiguration : IComparable<CompileConfiguration>, IComparable, IEquatable<CompileConfiguration>
    {
        /// <summary>Gets or sets the configuration name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the compiler flags.</summary>
        public string FlagFile { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should compile a folder or single file.</summary>
        public bool All { get; set; }

        /// <summary> Gets or sets a value indicating whether the compiler should suppress the output.</summary>
        public bool Quiet { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler should output debug information.</summary>
        public bool Debug { get; set; }

        /// <summary>Gets or sets a value indicating whether the compiler optimize should optimize the scripts.</summary>
        public bool Optimize { get; set; }

        /// <summary>Gets or sets the assembly option.</summary>
        public AssemblyOption AssemblyOption { get; set; }

        /// <summary>Gets or sets the import folders.</summary>
        public IEnumerable<ImportFolder> ImportFolders { get; set; }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.</returns>
        /// <param name="other">An object to compare with this object.</param>
        /// <remarks>This methods compares the order of two <see cref="CompileConfiguration">Compile Configurations</see>. However it does an ordering only by the name. This means, if two configurations have the same name, but differ in a property, they are
        /// considered the same by this method. Use the <see cref="Equals(SkyrimCompileHelper.Core.CompileConfiguration)"/> or <see cref="Equals(object)"/> method to check for true equality.
        /// </remarks>
        public int CompareTo(CompileConfiguration other)
        {
            return string.Compare(other.Name, this.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows <paramref name="obj"/> in the sort order. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception>
        public int CompareTo(object obj)
        {
            var configuration = obj as CompileConfiguration;
            return configuration != null ? this.CompareTo(configuration) : 1;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(CompileConfiguration other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.Name, other.Name) && string.Equals(this.FlagFile, other.FlagFile) && this.All == other.All && this.Quiet == other.Quiet && this.Debug == other.Debug && this.Optimize == other.Optimize && this.AssemblyOption == other.AssemblyOption && IEnumerable<ImportFolder>.Equals(this.ImportFolders, other.ImportFolders);
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var config = obj as CompileConfiguration;
            return config != null && this.Equals(config);
        }
        
        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref="CompileConfiguration"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Name != null ? this.Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.FlagFile != null ? this.FlagFile.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.All.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Quiet.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Debug.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Optimize.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.AssemblyOption;
                hashCode = (hashCode * 397) ^ (this.ImportFolders != null ? this.ImportFolders.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}