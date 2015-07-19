using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimCompilerHelper
{
    using System.Diagnostics;

    using SkyrimCompilerHelper.Options;

    /// <summary>Main program class.</summary>
    public class Program
    {
        private const string ConfigPath = @".\config";

        private static string invokedVerb;

        private static object invokedVerbInstance;


        /// <summary>Main entry point for the application.</summary>
        /// <param name="args">The arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            CommandOptions commandOptions = new CommandOptions();
            Utilities utils = new Utilities();

            if (!CommandLine.Parser.Default.ParseArguments(args, commandOptions, OnVerbCommand))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            switch (invokedVerb)
            {
                case "init":
                    Utilities.Initialize();
                    break;
                case "clean":
                    utils.Clean();
                    break;
            }


            Console.ReadLine();
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
