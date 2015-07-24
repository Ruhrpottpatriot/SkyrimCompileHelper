// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Robert Logiewa">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Robert Logiewa
// </copyright>
// <summary>
//   Main program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SkyrimCompileHelper
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    using IniParser;
    using IniParser.Model;

    using SkyrimCompileHelper.Options;

    /// <summary>Main program class.</summary>
    public class Program
    {
        private static string invokedVerb;

        private static object invokedVerbInstance;

        private static Utilities utils;

        /// <summary>Main entry point for the application.</summary>
        /// <param name="args">The arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            CommandOptions commandOptions = new CommandOptions();
            utils = new Utilities();

            if (!CommandLine.Parser.Default.ParseArguments(args, commandOptions, OnVerbCommand))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            switch (invokedVerb)
            {
                case "init":
                    Initialize((InitializeOption)invokedVerbInstance);
                    break;
                case "clean":
                    utils.Clean();
                    break;
                case "copy":
                    Copy((CopyOption)invokedVerbInstance);
                    break;
                case "compile":
                    {
                        Compiler compiler = new Compiler(((CompileOption)invokedVerbInstance).Mode);
                        compiler.Compile();
                    }

                    break;
                case "switch-compile-mode":
                    SwitchCompileMode();
                    break;
            }

            Console.Read();
        }


        private static void Copy(CopyOption copyOption)
        {
            if (copyOption.Assets)
            {
                string source = Path.Combine(utils.ConfigData["ProgramPaths"]["ModOrganizer"], "mods", utils.ConfigData["General"]["ModName"]);
                string destination = Path.Combine(utils.ConfigData["General"]["RepositoryPath"], "src");

                utils.Copy(source, destination);
            }
            else if (copyOption.Binary)
            {
                // Get the compile mode
                CompileMode mode;
                Enum.TryParse(utils.ConfigData["General"]["CompileMode"], out mode);

                string path = mode == CompileMode.Debug ? @"\bin\debug" : @"\bin\release";
                
                string source = Path.Combine(utils.ConfigData["General"]["RepositoryPath"], path);
                string destination = Path.Combine(utils.ConfigData["ProgramPaths"]["ModOrganizer"], "mods", utils.ConfigData["General"]["ModName"]);
                utils.Copy(source, destination);
            }
        }

        /// <summary>Initializes the modding environment.</summary>
        /// <param name="initializeOption">The initialize options.</param>
        private static void Initialize(InitializeOption initializeOption)
        {
            // Check if the config file exists already.
            if (File.Exists(@".\config.ini") && !initializeOption.Force)
            {
                Console.WriteLine("The config file already exists. If you want to reinitialize the environment use the 'force' parameter.");
                return;
            }
            else if (initializeOption.Force)
            {
                File.Delete(@".\config.ini");
            }

            Console.WriteLine("Fill out the following information to initialize the modding environment:");

            // General Information
            SectionData generalData = new SectionData("General");
            Console.Write("Mod Name: ");
            generalData.Keys.AddKey("ModName", Console.ReadLine());

            Console.Write("Repository Path (absolute): ");
            generalData.Keys.AddKey("RepositoryPath", Console.ReadLine());

            generalData.Keys.AddKey("CompileMode", CompileMode.Debug.ToString());

            // ProgramPaths
            SectionData pathData = new SectionData("ProgramPaths");
            Console.Write("Skyrim Path (absolute): ");
            pathData.Keys.AddKey("Skyrim", Console.ReadLine());
            Console.Write("Mod Organizer (absolute): ");
            pathData.Keys.AddKey("ModOrganizer", Console.ReadLine());

            // Compiler options
            SectionData compilerOptionsData = new SectionData("CompilerOptions");
            compilerOptionsData.LeadingComments.Add("Only change the section below, if you know what you are doing!");
            compilerOptionsData.Keys.AddKey("PapyrusCompiler", @"\Papyrus Compiler\PapyrusCompiler.exe");
            compilerOptionsData.Keys.AddKey("ScriptSourcePath", @"\Data\Scripts\Source");
            compilerOptionsData.Keys.AddKey("DefaultFlags", "\"{0}\" -all -f=\"TESV_Papyrus_Flags.flg\" -i=\"{1}\" -o=\"{2}\"");

            // Create a new ini file
            IniData config = new IniData();
            config.Sections.Add(generalData);
            config.Sections.Add(pathData);
            config.Sections.Add(compilerOptionsData);

            // Write the file to disk
            FileIniDataParser parser = new FileIniDataParser();
            parser.WriteFile(@".\config.ini", config, Encoding.UTF8);
        }
        private static void SwitchCompileMode()
        {
            throw new NotImplementedException();
        }

        /// <summary>Populates fields with parsed verbs.</summary>
        /// <param name="verb">The parsed verb.</param>
        /// <param name="verbInstance">The parsed verb instance.</param>
        private static void OnVerbCommand(string verb, object verbInstance)
        {
            invokedVerb = verb;
            invokedVerbInstance = verbInstance;
        }
    }
}
