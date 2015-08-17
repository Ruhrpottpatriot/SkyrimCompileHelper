// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Converts a <see cref="bool" /> into a corresponding <see cref="Visibility" /> value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Common.Converter
{
    using System.Windows;

    /// <summary>Converts a <see cref="bool"/> into a corresponding <see cref="Visibility"/> value.</summary>
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        /// <summary>Initialises a new instance of the <see cref="BooleanToVisibilityConverter"/> class.</summary>
        public BooleanToVisibilityConverter()
            : base(Visibility.Visible, Visibility.Collapsed)
        {
        }
    }
}