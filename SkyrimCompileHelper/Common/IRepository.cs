// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Provides the repository interface to retrieve items of type <see cref="TData" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Common
{
    using System.Collections.Generic;

    /// <summary>Provides the repository interface to retrieve items of type <see cref="TData"/>.</summary>
    /// <typeparam name="TData">The type of the data to retrieve.</typeparam>
    public interface IRepository<out TData>
    {
        /// <summary>Gets all items from the repository.</summary>
        /// <returns>An <see cref="IEnumerable{T}"/> containing all items in the repository.</returns>
        IEnumerable<TData> GetAll();

        /// <summary>Gets an item by its identifier.</summary>
        /// <returns>The item of type <see cref="TData"/>.</returns>
        TData GetByIdentfier();
    }
}
