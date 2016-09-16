using System;
using System.IO;
using System.Runtime.InteropServices;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Efficio
{
  [ComVisible(true)]
  [CodeGeneratorRegistration(typeof(EfficioGenerator), Name, VSConstants.UICONTEXT.CSharpProject_string, GeneratesDesignTimeSource = true)]
  [Guid("2564E090-89A5-451F-9390-688C3E6D7ED3")]
  [ProvideObject(typeof(EfficioGenerator))]
  public sealed class EfficioGenerator : BaseCodeGeneratorWithSite
  {
    private static readonly byte[] Empty = Array.Empty<byte>();
    internal const string Description = "Efficio Generator";
    internal const string Name = "Efficio";

    protected override byte[] GenerateCode(string content)
    {
      var projectItem = GetProjectItem();
      var scriptFilePath = projectItem.Properties.ItemOrNull($"{EfficioPackage.Name}.{ProjectItemMetadata.Script}")?.Value.ToString() ?? string.Empty;

      if (!String.IsNullOrWhiteSpace(scriptFilePath) && !Path.IsPathRooted(scriptFilePath))
      {
        scriptFilePath = Path.Combine(Helper.GetProjectFolder(projectItem.ContainingProject), scriptFilePath);
      }

      var scriptResult = File.Exists(scriptFilePath)
        ? Script.EvaluateAsync(File.ReadAllText(scriptFilePath), scriptFilePath, content).Result
        : Script.EvaluateAsync(content, projectItem.Properties.Item("FullPath").Value.ToString()).Result;

      foreach (var error in scriptResult.Errors)
      {
        GeneratorError(0, error.Message, (uint) error.Line, (uint) error.Column);
      }

      return null;
    }
  }
}
