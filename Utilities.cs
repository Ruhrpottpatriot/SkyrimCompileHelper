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
    using System.Text;

    using IniParser;
    using IniParser.Model;
    using IniParser.Parser;

    /// <summary>Contains some useful methods and properties.</summary>
    public class Utilities
    {
        /// <summary>Gets the config data.</summary>
        public IniData ConfigData { get; private set; }

        /// <summary>Initializes the modding environment</summary>
        public static void Initialize()
        {
            Console.WriteLine("Please enter the following information to initialize the modding environment.");

            // Create the general section
            SectionData generalSection = new SectionData("General");
            Console.Write("ModName: ");
            generalSection.Keys.AddKey("ModName", Console.ReadLine());
            
            // Create the program paths section
            SectionData pathsSection = new SectionData("ProgramPaths");
            Console.Write("Skyrim Path: ");
            pathsSection.Keys.AddKey("Skyrim", Console.ReadLine());
            Console.Write("Mod Organizer: ");
            pathsSection.Keys.AddKey("ModOrganizer", Console.ReadLine());
            
            // Generate the compiler options section
            SectionData compilerOptionsSection = new SectionData("CompilerOptions");
            compilerOptionsSection.LeadingComments.Add("Only change the section below, if you know what you are doing!");
            compilerOptionsSection.Keys.AddKey("PapyrusCompiler", @"\Papyrus Compiler\PapyrusCompiler.exe");
            compilerOptionsSection.Keys.AddKey("ScriptSourcePath", @"\Data\Scripts\Source");
            compilerOptionsSection.Keys.AddKey("DefaultFlags", "\"{0}\" -all -f=\"TESV_Papyrus_Flags.flg\" -i=\"{1}\" -o=\"{2}\"");
            
            // Create a new ini file
            IniData config = new IniData();
            config.Sections.Add(generalSection);
            config.Sections.Add(pathsSection);
            config.Sections.Add(compilerOptionsSection);

            // Write the file to disk
            FileIniDataParser parser = new FileIniDataParser();
            parser.WriteFile(@".\config.ini", config, Encoding.UTF8); 
        }

        /// <summary>Deletes files and folders from the build directory and mod organizer directory.a</summary>
        public void Clean()
        {
            string modName = this.ConfigData["General"]["ModName"];
            string organizerPath = this.ConfigData["ProgramPaths"]["ModOrganizer"];
            DirectoryInfo organizerDirectoryInfo = new DirectoryInfo(Path.Combine(organizerPath, "mods", modName));
            this.DeleteFiles(organizerDirectoryInfo);
            this.DeleteDirectories(organizerDirectoryInfo);

            DirectoryInfo buildDebugDirectoryInfo = new DirectoryInfo(@"..\src\debug");
            this.DeleteFiles(buildDebugDirectoryInfo);
            this.DeleteDirectories(buildDebugDirectoryInfo);


            DirectoryInfo buildReleaseDirectoryInfo = new DirectoryInfo(@"..\src\release");
            this.DeleteFiles(buildReleaseDirectoryInfo);
            this.DeleteDirectories(buildReleaseDirectoryInfo);
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