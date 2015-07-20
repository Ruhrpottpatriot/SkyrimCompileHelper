using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimCompilerHelper
{
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    using SkyrimCompilerHelper.Options;

    /// <summary>Main program class.</summary>
    public class Program
    {
        private const string ConfigPath = @".\config";

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
                    Utilities.Initialize((InitializeOption)invokedVerbInstance);
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

            }

            Console.Read();
        }

        private static void Copy(CopyOption copyOption)
        {
            if (copyOption.Assets)
            {
                string source = Path.Combine(utils.ConfigData["ProgramPaths"]["ModOrganizer"], "mods", utils.ConfigData["General"]["ModName"]);
                string destination = @"..\src";

                utils.Copy(source, destination);
            }
            else if (copyOption.Binary)
            {
                string source = @".\bin\debug";
                string destination = Path.Combine(utils.ConfigData["ProgramPaths"]["ModOrganizer"], "mods", utils.ConfigData["General"]["ModName"]);
                utils.Copy(source, destination);
            }
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
