// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISettingsRepository.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Represents the settings repository, containing methods to manipulate the application settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Repositories
{
    /// <summary>Represents the settings repository, containing methods to manipulate the application settings.</summary>
    public interface ISettingsRepository : IRepository<string, object>
    {
    }
}