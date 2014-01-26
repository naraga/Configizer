using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace ConfigizerLib.Compilation
{
    public class ConfigurationCompilationException : Exception
    {
        public ConfigurationCompilationException(ConfigurationFileInfo configuration, IEnumerable<CompilerError> compilationErrors)
        {
            _configuration = configuration;
            _compilationErrors = compilationErrors;
        }

        public ConfigurationCompilationException(ConfigurationFileInfo configuration, CompilerErrorCollection compilationErrors)
            : this(configuration, compilationErrors.Cast<CompilerError>())
        {
        }

        private readonly ConfigurationFileInfo _configuration;
        private readonly IEnumerable<CompilerError> _compilationErrors;

        public ConfigurationFileInfo Configuration
        {
            get { return _configuration; }
        }

        public IEnumerable<CompilerError> CompilationErrors
        {
            get { return _compilationErrors; }
        }

        public override string Message
        {
            get
            {
                return string.Format(
                    @"Compilation errors processing the file {0} or one of its references.{1}",
                    Configuration.Name,
                    _compilationErrors.Aggregate(
                        "",
                        (acc, e) => string.Format("\n{0}(Line {1}):{2}", acc, e.Line, e.ErrorText))
                    );
            }
        }
    }
}