using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimCompileHelper.Compiler
{
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Media;

    public class Compiler
    {
        private readonly string compilerPath;

        public Compiler(string skyrimPath)
        {
            this.compilerPath = Path.Combine(skyrimPath, @"Papyrus Compiler\PapyrusCompiler.exe");
        }

        public string Flags { get; set; }

        public IEnumerable<string> InputFolders { get; set; }

        public string OutputFolder { get; set; }

        public void Compile()
        {
            if (string.IsNullOrWhiteSpace(this.Flags))
            {
                throw new CompilerFlagsException("The flags passed to the compiler must not be an empty string.");
            }

            if (this.InputFolders == null || !this.InputFolders.Any())
            {
                throw new CompilerFolderException("The folders passed to the compiler must not be empty.");
            }

            if (string.IsNullOrWhiteSpace(this.OutputFolder))
            {
                throw new CompilerFolderException("The output folder has to be specified.");
            }

            Process process = new Process
            {
                StartInfo =
                {
                    FileName = this.compilerPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };
            process.ErrorDataReceived += this.ErrorDataRecieved;
            process.OutputDataReceived += this.DataReceived;

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();
        }

        private void ErrorDataRecieved(object sender, DataReceivedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void DataReceived(object sender, DataReceivedEventArgs args)
        {
            
        }
    }
}
