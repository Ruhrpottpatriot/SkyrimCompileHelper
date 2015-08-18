// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumListExtension.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Markup extension that provides a list of the members of a given enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Markup;

    /// <summary>Markup extension that provides a list of the members of a given enum.</summary>
    public class EnumListExtension : MarkupExtension
    {
        /// <summary>The enum type.</summary>
        private Type enumType;

        /// <summary>The as string.</summary>
        private bool asString;

        /// <summary>Initialises a new instance of the <see cref="EnumListExtension"/> class.</summary>
        public EnumListExtension()
        {
        }

        /// <summary>Initialises a new instance of the <see cref="EnumListExtension"/> class. </summary>
        /// <param name="enumType">The type of enum whose members are to be returned.</param>
        public EnumListExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        /// <summary>Gets or sets the type of enumeration to return</summary>
        public Type EnumType
        {
            get
            {
                return this.enumType;
            }

            set
            {
                if (value != this.enumType)
                {
                    if (null != value)
                    {
                        Type underlyingType = Nullable.GetUnderlyingType(value) ?? value;

                        if (underlyingType.IsEnum == false)
                        {
                            throw new ArgumentException("Type must be for an Enum.");
                        }
                    }

                    this.enumType = value;
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether to display the enumeration members as strings using the Description on the member if available.</summary>
        public bool AsString
        {
            get
            {
                return this.asString;
            }

            set
            {
                this.asString = value;
            }
        }

        /// <summary>Returns a list of items for the specified <see cref="EnumType"/>.</summary>
        /// <param name="serviceProvider">An object that provides services for the markup extension.</param>
        /// <returns>A list of items.</returns>
        /// <remarks>
        /// This function returns a list of items for the specified <see cref="EnumType"/>. Depending on the <see cref="AsString"/> property, the
        /// items will be returned as the enum member value or as strings.
        /// </remarks>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this.enumType)
            {
                throw new InvalidOperationException("The EnumType must be specified.");
            }

            Type actualEnumType = Nullable.GetUnderlyingType(this.enumType) ?? this.enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            // if the object itself is to be returned then just use GetValues
            if (this.asString == false)
            {
                if (actualEnumType == this.enumType)
                {
                    return enumValues;
                }

                Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
                enumValues.CopyTo(tempArray, 1);
                return tempArray;
            }

            var items = new List<string>();

            if (actualEnumType != this.enumType)
            {
                items.Add(null);
            }

            // otherwise we must process the list
            foreach (object item in Enum.GetValues(this.enumType))
            {
                string itemString = item.ToString();
                FieldInfo field = this.enumType.GetField(itemString);
                object[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attribs.Length > 0)
                {
                    itemString = ((DescriptionAttribute)attribs[0]).Description;
                }

                items.Add(itemString);
            }

            return items.ToArray();
        }
    }
}
