// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportFolder.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents an import folder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Core
{
    using System;
    using System.IO;

    using PropertyChanged;

    /// <summary>Represents an import folder.</summary>
    [ImplementPropertyChanged]
    public class ImportFolder : IComparable<ImportFolder>, IComparable, IEquatable<ImportFolder>
    {
        /// <summary>Initialises a new instance of the <see cref="ImportFolder"/> class. </summary>
        /// <param name="path">The path to the import folder.</param>
        public ImportFolder(string path)
        {
            this.FolderPath = path;
            this.Name = string.IsNullOrWhiteSpace(path) ? string.Empty : Path.GetFileName(Path.GetFullPath(this.FolderPath).TrimEnd(Path.DirectorySeparatorChar));
        }

        /// <summary>Initialises a new instance of the <see cref="ImportFolder"/> class. </summary>
        /// <param name="path">The path to the import folder.</param>
        /// <param name="name">The display name of the import folder.</param>
        public ImportFolder(string path, string name)
        {
            this.FolderPath = path;
            this.Name = name;
        }

        /// <summary>Gets or sets the folder name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the path of the current <see cref="ImportFolder"/>.</summary>
        public string FolderPath { get; set; }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ImportFolder other)
        {
            int pathComparison = string.Compare(this.FolderPath, other.FolderPath, StringComparison.OrdinalIgnoreCase);

            return pathComparison == 0 ? string.Compare(this.Name, other.FolderPath, StringComparison.OrdinalIgnoreCase) : pathComparison;
        }

        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj"/> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj"/>. Greater than zero This instance follows <paramref name="obj"/> in the sort order. </returns>
        /// <param name="obj">An object to compare with this instance. </param><exception cref="T:System.ArgumentException"><paramref name="obj"/> is not the same type as this instance. </exception>
        public int CompareTo(object obj)
        {
            var folder = obj as ImportFolder;
            return folder != null ? this.CompareTo(folder) : 1;
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>True if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ImportFolder other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.Name, other.Name) && string.Equals(this.FolderPath, other.FolderPath);
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="ImportFolder"/>.</summary>
        /// <returns>True if the specified object  is equal to the current object; otherwise, false.</returns>
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

            var folder = obj as ImportFolder;
            return folder != null && this.Equals(folder);
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref="ImportFolder"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Name != null ? this.Name.GetHashCode() : 0) * 397) ^ (this.FolderPath != null ? this.FolderPath.GetHashCode() : 0);
            }
        }
    }
}
