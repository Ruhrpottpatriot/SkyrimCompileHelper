// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Contains some useful methods and properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompilerHelper
{
    using System;
    using System.IO;

    using IniParser;
    using IniParser.Model;

    /// <summary>Contains some useful methods and properties.</summary>
    public class Utilities
    {
        /// <summary>Gets the config data.</summary>
        public IniData ConfigData { get; private set; }

        /// <summary>Initializes the modding environment</summary>
        public void Initialize()
        {




            string keys = Console.ReadLine();

            Console.WriteLine(keys);


            //string organiserPath = this.ConfigData["ProgramPaths"]["ModOrganizer"];

            //string organizerModDir = Path.Combine(organiserPath, @"\mods");

            //if (Directory.Exists(organizerModDir))
            //{
            //    return;
            //}

            //Directory.CreateDirectory(organizerModDir);
        }

        public void Clean()
        {
            string modName = this.ConfigData["General"]["ModName"];

            string organizerPath = this.ConfigData["ProgramPaths"]["ModOrganizer"];
            
            DirectoryInfo organizerDirectoryInfo = new DirectoryInfo(Path.Combine(organizerPath, "mods", modName));

            this.DeleteFiles(organizerDirectoryInfo);
            this.DeleteDirectories(organizerDirectoryInfo);

            DirectoryInfo builDirectoryInfo = new DirectoryInfo(@"..\src");

        }

        /// <summary>Deletes the sub directories in a folder.</summary>
        /// <param name="dirInfo">The folder to delete the sub folders.</param>
        private void DeleteDirectories(DirectoryInfo dirInfo)
        {
            try
            {
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>Deletes files in the folder.</summary>
        /// <param name="dirInfo">The folder to delete the files.</param>
        private void DeleteFiles(DirectoryInfo dirInfo)
        {
            try
            {
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}