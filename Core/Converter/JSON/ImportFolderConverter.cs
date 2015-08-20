// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportFolderConverter.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Converts a collection of <see cref="ImportFolder" /> into a collection of strings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Core.JSON
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    /// <summary>Converts a collection of <see cref="ImportFolder"/> into a collection of strings.</summary>
    public class ImportFolderConverter : JsonConverter
    {
        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serialiser.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var folders = value as IEnumerable<ImportFolder>;

            if (folders != null)
            {
                var paths = folders.Select(f => f.FolderPath);

                serializer.Serialize(writer, paths);
            }
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serialiser.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IEnumerable<string> obj = serializer.Deserialize<IEnumerable<string>>(reader);

            return obj.Select(path => new ImportFolder { FolderPath = path });
        }

        /// <summary>Determines whether this instance can convert the specified object type.</summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
