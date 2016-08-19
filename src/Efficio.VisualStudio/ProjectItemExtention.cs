// ReSharper disable SuspiciousTypeConversion.Global

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using EnvDTE;

namespace Efficio
{
  /// <summary>
  /// Adds "Custom Tool Script" and "Custom Tool Parameters" properties to the C# and VB.NET project item properties.
  /// </summary>
  [ComVisible(true)]
  public partial class ProjectItemExtention
  {
    private readonly int _cookie;
    private readonly ProjectItem _projectItem;
    private readonly uint _projectItemID;
    private readonly IVsBuildPropertyStorage _propertyStorage;
    private readonly IExtenderSite _site;

    /// <summary>
    /// Gets or sets the file extenderName of the template used by the <see cref="CustomTool"/>.
    /// </summary>
    [Category(EfficioPackage.Name)]
    [DisplayName(CustomTool.Name + " " + ProjectItemMetadata.Script)]
    [Description("An optional C# Script is executed by the " + CustomTool.Name + "(Custom Tool) instead of the input-file itself.")]
    [Editor(typeof(ScriptFileNameEditor), typeof(UITypeEditor))]
    public string Script
    {
      get
      {
        var value = default(string);

        if (ErrorHandler.Failed(_propertyStorage.GetItemAttribute(_projectItemID, ProjectItemMetadata.Script, out value)))
        {
          value = string.Empty;
        }

        return value;
      }
      set
      {
        var customToolProperty = _projectItem.Properties.Item(ProjectItemProperty.CustomTool);
        var customToolValue = customToolProperty.Value as string;

        if (!string.IsNullOrWhiteSpace(value))
        {
          // Report an error if the user tries to specify template for an incompatible custom tool.
          if (!string.IsNullOrWhiteSpace(customToolValue) && customToolValue != CustomTool.Name)
          {
            throw new InvalidOperationException($"The '{ProjectItemProperty.CustomTool}' property is supported only by the '{CustomTool.Name}'-Custom Tool. Set the 'Custom Tool' property first.");
          }
        }

        ErrorHandler.ThrowOnFailure(_propertyStorage.SetItemAttribute(_projectItemID, ProjectItemMetadata.Script, value));

        // If the file does not have a custom tool yet, assume that by specifying the template user wants to use the T4Toolbox.TemplatedFileGenerator.
        if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(customToolValue))
        {
          _projectItem.Properties.Item(ProjectItemProperty.CustomTool).Value = CustomTool.Name;
        }
      }
    }

    //[Category(EfficioPackage.Name)]
    //[DisplayName(CustomTool.Name + " " + ProjectItemMetadata.References)]
    //[Description("References used by the " + CustomTool.Name + "(Custom Tool) to execute code.")]
    //public string[] References
    //{
    //  get
    //  {
    //    return Array.Empty<string>();
    //  }
    //  set
    //  {
    //  }
    //}

    internal ProjectItemExtention(IServiceProvider serviceProvider, IVsBrowseObject browseObject, IExtenderSite site, int cookie)
    {
      if (serviceProvider == null)
      {
        throw new ArgumentNullException(nameof(serviceProvider));
      }

      if (browseObject == null)
      {
        throw new ArgumentNullException(nameof(browseObject));
      }

      if (site == null)
      {
        throw new ArgumentNullException(nameof(site));
      }

      var project = default(IVsHierarchy);
      var projectItemObject = default(object);

      ErrorHandler.ThrowOnFailure(browseObject.GetProjectItem(out project, out _projectItemID));
      ErrorHandler.ThrowOnFailure(project.GetProperty(_projectItemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out projectItemObject));

      _cookie = cookie;
      _projectItem = (ProjectItem) projectItemObject;
      _propertyStorage = (IVsBuildPropertyStorage) project;
      _site = site;
    }

    ~ProjectItemExtention()
    {
      try
      {
        _site.NotifyDelete(_cookie);
      }
      catch (InvalidComObjectException)
      {
      }
    }
  }
}