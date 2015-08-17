// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanConverter.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Converts two values of type <see cref="TValue" /> into a corresponding <see cref="bool" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>Converts two values of type <see cref="TValue"/> into a corresponding <see cref="bool"/>.</summary>
    /// <typeparam name="TValue">The type of the value to convert into a <see cref="bool"/>.</typeparam>
    public class BooleanConverter<TValue> : IValueConverter
    {
        /// <summary>Initialises a new instance of the <see cref="BooleanConverter{TValue}"/> class.</summary>
        /// <param name="trueValue">The true value.</param>
        /// <param name="falseValue">The false value.</param>
        public BooleanConverter(TValue trueValue, TValue falseValue)
        {
            this.True = trueValue;
            this.False = falseValue;
        }

        /// <summary>Gets or sets the true value.</summary>
        public TValue True { get; set; }

        /// <summary> Gets or sets the false value.</summary>
        public TValue False { get; set; }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && ((bool)value) ? this.True : this.False;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is TValue && EqualityComparer<TValue>.Default.Equals((TValue)value, this.True);
        }
    }
}
