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

namespace SkyrimCompileHelper.Common
{
    /// <summary>Provides the interface for the settings repository.</summary>
    public interface ISettingsRepository : IRepository<string, object>
    {
    }
}