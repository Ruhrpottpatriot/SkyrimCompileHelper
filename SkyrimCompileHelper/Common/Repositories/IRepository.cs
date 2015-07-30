// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Provides the interface for data sources.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Common
{
    using System.Collections.Generic;

    /// <summary>Provides the interface for data sources.</summary>
    /// <typeparam name="TKey">The type of the key values that uniquely identify the entities in the repository.</typeparam>
    /// <typeparam name="TValue">The type of the entities in the repository.</typeparam>
    public interface IRepository<TKey, TValue>
    {
        /// <summary>Creates a new set of items inside the repository.</summary>
        /// <param name="items">An <see cref="IDictionaryRange{TKey,TValue}"/> of items to add to the repository.</param>
        void Create(IDictionaryRange<TKey, TValue> items);

        /// <summary>Reads the complete repository and returns it to the user.</summary>
        /// <returns>An <see cref="IDictionaryRange{TKey,TValue}"/> containing all items in the repository.</returns>
        IDictionaryRange<TKey, TValue> Read();

        /// <summary>Updates a set of items in the repository.</summary>
        /// <param name="items">An <see cref="IDictionaryRange{TKey,TValue}"/> containing the items to update.</param>
        void Update(IDictionaryRange<TKey, TValue> items);

        /// <summary>Deletes a set of items from the repository.</summary>
        /// <param name="identifiers">An <see cref="IEnumerable{T}"/> listing the identifiers of the items to delete.</param>
        void Delete(IEnumerable<TKey> identifiers);
    }
}