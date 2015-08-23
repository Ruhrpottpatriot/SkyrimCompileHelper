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

namespace SkyrimCompileHelper.Core
{
    using System;
    using System.Collections.Generic;

    using PropertyChanged;

    using Semver;

    /// <summary>Represents a solution.</summary>
    [ImplementPropertyChanged]
    public class Solution : IComparable<Solution>, IComparable, IEquatable<Solution>
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

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.</returns>
        /// <param name="other">An object to compare with this object.</param>
        /// <remarks>This method compares two <see cref="Solution">Solutions</see> by their <see cref="Name"/> and <see cref="SemVersion">Version</see>. Compile configurations are not compared, since they have no influence on the order of two solutions.
        /// Therefore if two solutions have the same name and version they are considered equal. The comparison first compares the Name via <see cref="StringComparison.OrdinalIgnoreCase"/> and then the version by calling <see cref="SemVersion.CompareTo(Semver.SemVersion)"/>.
        /// Also, if the other solution is null, the current solution is treated as greater than the other solution.
        /// </remarks>
        public int CompareTo(Solution other)
        {
            if (other == null)
            {
                return 1;
            }

            int nameOrder = string.Compare(other.Name, this.Name, StringComparison.OrdinalIgnoreCase);

            return nameOrder == 0 ? other.Version.CompareTo(this.Version) : nameOrder;
        }

        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows <paramref name="obj"/> in the sort order.</returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception>
        public int CompareTo(object obj)
        {
            Solution sln = obj as Solution;
            if (sln != null)
            {
                return this.CompareTo(sln);
            }

            return 1;
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>True if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Solution other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.Name, other.Name) && IList<CompileConfiguration>.Equals(this.CompileConfigurations, other.CompileConfigurations) && SemVersion.Equals(this.Version, other.Version);
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.</summary>
        /// <returns>True if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
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

            Solution solution = obj as Solution;
            return solution != null && this.Equals(solution);
        }

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current <see cref="Solution"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Name != null ? this.Name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.CompileConfigurations != null ? this.CompileConfigurations.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Version != null ? this.Version.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}