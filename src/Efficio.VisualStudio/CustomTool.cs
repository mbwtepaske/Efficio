﻿using System.IO;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace Efficio
{
  [CodeGeneratorRegistration(typeof(CustomTool), Description, VSConstants.UICONTEXT.CSharpProject_string, GeneratesDesignTimeSource = true)]
  public sealed class CustomTool : BaseCodeGeneratorWithSite
  {
    internal const string Description = "Efficio Generator";
    internal const string Name = "Efficio";
    
    protected override byte[] GenerateCode(string content)
    {
      var projectItemExtender = GetProjectItem().Extender[EfficioPackage.Name] as ProjectItemExtention;

      if (projectItemExtender != null)
      {
        var scriptFile = projectItemExtender.Script;

        if (File.Exists(scriptFile))
        {
          var scriptCode = File.ReadAllText(scriptFile);
          var scriptResult = Script<string>.EvaluateAsync(scriptCode, scriptFile, content).Result;
          
          
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
