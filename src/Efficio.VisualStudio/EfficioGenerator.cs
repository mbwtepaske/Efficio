using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace Efficio
{
  [ComVisible(true)]
  [CodeGeneratorRegistration(typeof(EfficioGenerator), Name, VSConstants.UICONTEXT.CSharpProject_string, GeneratesDesignTimeSource = true)]
  [Guid("2564E090-89A5-451F-9390-688C3E6D7ED3")]
  [ProvideObject(typeof(EfficioGenerator))]
  public sealed class EfficioGenerator : BaseCodeGeneratorWithSite
  {
    internal const string Description = "Efficio Generator";
    internal const string Name = "Efficio";
    
    protected override byte[] GenerateCode(string content)
    {
      //var projectItemExtender = GetProjectItem().Extender[EfficioPackage.Name] as ProjectItemExtension;

      //if (projectItemExtender != null)
      {
        var scriptFile = GetProjectItem().Properties.Item(ProjectItemMetadata.Script).Value.ToString() ?? String.Empty;// projectItemExtender.Script;

        if (File.Exists(scriptFile))
        {
          var scriptCode = File.ReadAllText(scriptFile);
          var scriptResult = Script.EvaluateAsync(scriptCode, scriptFile, content).Result;

          foreach (var error in scriptResult.Errors)
          {
            GeneratorError(0, error.Message, (uint) error.Line, (uint) error.Column);
          }
        }
      }

      return null;
    }

    //private byte[] GenerateFromAssociatedTemplateFile(string content)
    //{
    //  var hierarchy = SiteServiceProvider.GetService<IVsHierarchy>();
    //  uint inputItemId;
    //
    //  ErrorHandler.ThrowOnFailure(hierarchy.ParseCanonicalName(content, out inputItemId));
    //
    //  var templatePath = default(string);
    //  var propertyStorage = SiteServiceProvider.GetService< IVsBuildPropertyStorage>();
    //  
    //  if (ErrorHandler.Failed(propertyStorage.GetItemAttribute(inputItemId, ProjectItemMetadata.Template, out templatePath)))
    //  {
    //    return null;
    //  }
    //
    //  // Remove <Template> metadata from the project item and refresh the properties window
    //  ErrorHandler.ThrowOnFailure(propertyStorage.SetItemAttribute(inputItemId, ProjectItemMetadata.Template, null));
    //
    //  var propertyBrowser = (IVSMDPropertyBrowser) GlobalServiceProvider.GetService(typeof(SVSMDPropertyBrowser));
    //
    //  propertyBrowser.Refresh();
    //
    //  var templateLocator = (TemplateLocator) GlobalServiceProvider.GetService(typeof(TemplateLocator));
    //
    //  return templateLocator.LocateTemplate(content, ref templatePath)
    //    ? File.ReadAllBytes(templatePath)
    //    : null;
    //}
  }
}
