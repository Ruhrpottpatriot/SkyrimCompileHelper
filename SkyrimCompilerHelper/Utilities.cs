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

namespace SkyrimCompileHelper
{
    using System;
    using System.IO;
    using System.Text;

    using IniParser;
    using IniParser.Model;

    using SkyrimCompileHelper.Options;

    /// <summary>Contains some useful methods and properties.</summary>
    public class Utilities
    {
        /// <summary>Initialises a new instance of the <see cref="Utilities"/> class.</summary>
        public Utilities()
        {
            this.ConfigData = new FileIniDataParser().ReadFile(@".\config.ini");
        }

        /// <summary>Gets the config data.</summary>
        public IniData ConfigData { get; private set; }

        /// <summary>Initializes the modding environment</summary>
        /// <param name="invokedVerbInstance">The verb options.</param>
        public static void Initialize(InitializeOption invokedVerbInstance)
        {
            if (File.Exists(@".\config.ini") && !invokedVerbInstance.Force)
            {
                Console.WriteLine("The environment has already been initialized. Use the -f parameter to reinitialize");
                return;
            }
            else if (invokedVerbInstance.Force)
            {
                File.Delete(@".\config.ini");
            }

            Console.WriteLine("Please enter some information to initialize the modding environment.");

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
            this.CleanDirectory(new DirectoryInfo(Path.Combine(this.ConfigData["ProgramPaths"]["ModOrganizer"], "mods", this.ConfigData["General"]["ModName"])));
            this.CleanDirectory(new DirectoryInfo(@"..\src\debug"));
            this.CleanDirectory(new DirectoryInfo(@"..\src\release"));
        }

        /// <summary>Copies the contents of a directory from one location to another.</summary>
        /// <param name="source">The source directory.</param>
        /// <param name="destination">The destination directory.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown, when the source directory does not exist.</exception>
        public void Copy(string source, string destination)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(source);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + source);
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destination, file.Name);
                file.CopyTo(temppath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destination, subdir.Name);
                this.Copy(subdir.FullName, temppath);
            }
        }

        private void CleanDirectory(DirectoryInfo dirInfo)
        {
            this.DeleteFiles(dirInfo);
            this.DeleteDirectories(dirInfo);
        }

        /// <summary>Deletes the sub directories in a folder.</summary>
        /// <param name="dirInfo">The folder to delete the sub folders.</param>
        private void DeleteDirectories(DirectoryInfo dirInfo)
        {
            try
            {
                if (dirInfo.Exists)
                {
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        file.Delete();
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>Deletes files in the folder.</summary>
        /// <param name="dirInfo">The folder to delete the files.</param>
        private void DeleteFiles(DirectoryInfo dirInfo)
        {
            try
            {
                if (dirInfo.Exists)
                {
                    foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}