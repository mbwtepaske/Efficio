using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Efficio
{
  /// <summary>
  /// A managed wrapper for VS's concept of an IVsSingleFileGenerator which is
  /// a custom tool invoked at design time which can take any file as an input
  /// and provide any file as outputFileSize.
  /// </summary>
  public abstract class BaseCodeGenerator : IVsSingleFileGenerator
  {
    #region IVsSingleFileGenerator Members

    /// <summary>
    /// Returns the extension of the generated file.
    /// </summary>
    /// <param name="defaultExtension">Out parameter, will hold the extension that is to be given to the outputFileSize file name. The returned extension must include a leading period</param>
    int IVsSingleFileGenerator.DefaultExtension(out string defaultExtension)
    {
      defaultExtension = string.Empty;

      try
      {
        defaultExtension = GetDefaultExtension();

        return VSConstants.S_OK;
      }
      catch (Exception exception)
      {
        Trace.WriteLine($"{nameof(IVsSingleFileGenerator.DefaultExtension)} failed. {exception}");
        
        return VSConstants.E_FAIL;
      }
    }

    /// <summary>
    /// Executes the transformation and returns the newly generated outputFileSize file, whenever a custom tool is loaded, or the input file is saved
    /// </summary>
    int IVsSingleFileGenerator.Generate(string filePath, string fileContent, string defaultNamespace, IntPtr[] outputFileData, out uint outputFileSize, IVsGeneratorProgress progress)
    {
      if (fileContent == null)
      {
        throw new ArgumentNullException(nameof(fileContent));
      }

      FilePath = filePath;
      FileNameSpace = defaultNamespace;
      Progress = progress;

      var bytes = GenerateCode(fileContent);

      if (bytes != null)
      {
        outputFileData[0] = Marshal.AllocCoTaskMem(bytes.Length);

        Marshal.Copy(bytes, 0, outputFileData[0], bytes.Length);

        outputFileSize = (uint)bytes.Length;

        return VSConstants.S_OK;
      }
      
      outputFileData = null;
      outputFileSize = 0;

      return VSConstants.E_FAIL;
    }

    #endregion

    /// <summary>
    /// Gets the namespace for the file
    /// </summary>
    protected string FileNameSpace
    {
      get;
      private set;
    } = string.Empty;

    /// <summary>
    /// File path of the input file
    /// </summary>
    protected string FilePath
    {
      get;
      private set;
    } = string.Empty;

    /// <summary>
    /// Interface to the VS shell object we use to tell our progress while we are generating
    /// </summary>
    internal IVsGeneratorProgress Progress
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the default extension for this generator
    /// </summary>
    /// <returns>String with the default extension for this generator</returns>
    protected abstract string GetDefaultExtension();

    /// <summary>
    /// The method that does the actual work of generating code given the input file
    /// </summary>
    protected abstract byte[] GenerateCode(string content);

    /// <summary>
    /// Reports an error via the shell callback mechanism.
    /// </summary>
    protected virtual void GeneratorError(uint level, string message, uint line, uint column) => Progress?.GeneratorError(0, level, message, line, column);

    /// <summary>
    /// Reports an warning via the shell callback mechanism.
    /// </summary>
    protected virtual void GeneratorWarning(uint level, string message, uint line, uint column) => Progress?.GeneratorError(1, level, message, line, column);
  }
}
