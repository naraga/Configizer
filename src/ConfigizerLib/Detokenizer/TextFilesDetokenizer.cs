using System;
using System.IO;
using System.Linq;

namespace ConfigizerLib.Detokenizer
{
    static public class TextFilesDetokenizer
    {
        public static Func<string[], Func<string, string>> RemoveExtensionOutputFileNameSelector =
            extensionsToRemove => delegate(string inputFileName)
            {
                var ext = Path.GetExtension(inputFileName) ?? string.Empty;
                if (string.Empty != ext && extensionsToRemove.Contains(ext))
                    return
                        inputFileName.Remove(inputFileName.LastIndexOf(ext, StringComparison.InvariantCultureIgnoreCase));

                return inputFileName;
            }; 

        public static void Detokenize(object tokensSource,
            string baseDir, string[] paths, 
            string[] fileNameSearchPatterns, Func<string, string> outputFileNameSelctor,
            bool forceOverwriteIfReadonly = true)
        {
            var files = new FilesEnumerator(paths, fileNameSearchPatterns, baseDir).GetFileNames();
            foreach (var fileName in files)
            {
                var outputFileName = outputFileNameSelctor(fileName);

                var outputFileExists = File.Exists(outputFileName);
                var outputFileIsReadonly = outputFileExists && (File.GetAttributes(outputFileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                if (outputFileExists && outputFileIsReadonly && !forceOverwriteIfReadonly)
                    throw new Exception("File " + outputFileName + " is readonly.");

                var fileContents = File.ReadAllText(fileName);
                var detokenizedContents = tokensSource.Interpolate(fileContents);

                // writes to the file if content has changed - idea is to avoid unnecessarily touching files which can lead e.g. to unnecessary recompilation performed by MSBuild
                if(!outputFileExists || fileContents != detokenizedContents)
                    File.WriteAllText(outputFileName, detokenizedContents);
            }
        }
    }
}
