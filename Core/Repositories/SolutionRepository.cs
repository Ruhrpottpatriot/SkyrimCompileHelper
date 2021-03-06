﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionRepository.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Contains methods and properties to manipulate solutions used by the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    /// <summary>Contains methods and properties to manipulate solutions used by the application.</summary>
    public class SolutionRepository : ISolutionRepository
    {
        /// <summary>The solution path.</summary>
        private readonly string solutionPath;

        /// <summary>Initialises a new instance of the <see cref="SolutionRepository"/> class.</summary>
        public SolutionRepository()
        {
            this.solutionPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SCH\Solutions");
        }

        /// <summary>Creates a new set of items inside the repository.</summary>
        /// <param name="items">An <see cref="IDictionaryRange{TKey,TValue}"/> of items to add to the repository.</param>
        public void Create(IDictionaryRange<string, Solution> items)
        {
            if (!Directory.Exists(this.solutionPath))
            {
                Directory.CreateDirectory(this.solutionPath);
            }

            foreach (KeyValuePair<string, Solution> pair in items)
            {
                using (StreamWriter writer = File.CreateText(Path.Combine(this.solutionPath, pair.Key + ".smsln")))
                {
                    using (JsonWriter jsonWriter = new JsonTextWriter(writer))
                    {
                        jsonWriter.Formatting = Formatting.Indented;

                        new JsonSerializer().Serialize(jsonWriter, pair.Value);
                    }
                }
            }
        }

        /// <summary>Reads the complete repository and returns it to the user.</summary>
        /// <returns>An <see cref="IDictionaryRange{TKey,TValue}"/> containing all items in the repository.</returns>
        public IDictionaryRange<string, Solution> Read()
        {
            if (!Directory.Exists(this.solutionPath))
            {
                return new DictionaryRange<string, Solution>(0);
            }

            string[] files = Directory.GetFiles(this.solutionPath);

            var solutions = new DictionaryRange<string, Solution>();
            foreach (string file in files)
            {
                using (StreamReader reader = File.OpenText(file))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    var solution = serializer.Deserialize<Solution>(new JsonTextReader(reader));

                    solutions.Add(solution.Name, solution);
                }
            }

            return solutions;
        }

        /// <summary>Updates a set of items in the repository.</summary>
        /// <param name="items">An <see cref="IDictionaryRange{TKey,TValue}"/> containing the items to update.</param>
        public void Update(IDictionaryRange<string, Solution> items)
        {
            this.Delete(items.Keys);

            this.Create(items);
        }

        /// <summary>Deletes a set of items from the repository.</summary>
        /// <param name="identifiers">An <see cref="IEnumerable{T}"/> listing the identifiers of the items to delete.</param>
        public void Delete(IEnumerable<string> identifiers)
        {
            if (!Directory.Exists(this.solutionPath))
            {
                return;
            }

            if (identifiers == null)
            {
                return;
            }

            foreach (string solutionPath in identifiers.Select(identifier => Path.Combine(this.solutionPath, identifier + ".smsln")).Where(File.Exists))
            {
                File.Delete(solutionPath);
            }
        }
    }
}