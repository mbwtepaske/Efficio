// ReSharper disable SuspiciousTypeConversion.Global

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using EnvDTE;

using IExtenderProvider = EnvDTE.IExtenderProvider;

using VSLangProj;

namespace Efficio
{
  /// <summary>
  /// Adds "Custom Tool Template" and "Custom Tool Parameters" properties to the C# and VB.NET project item properties.
  /// </summary>
  [ComVisible(true)]
  public class ProjectItemExtender
  {
    private class Provider : IExtenderProvider
    {
      private readonly string _categoryID;
      private readonly IServiceProvider _serviceProvider;

      public Provider(IServiceProvider serviceProvider, string categoryID)
      {
        _categoryID = categoryID;
        _serviceProvider = serviceProvider;
      }

      public bool CanExtend(string categoryID, string extenderName, object extendee) => categoryID == _categoryID && extenderName == EfficioPackage.Name;

      public object GetExtender(string categoryID, string name, object extendee, IExtenderSite extenderSite, int cookie) 
        => new ProjectItemExtender(_serviceProvider, (IVsBrowseObject) extendee, extenderSite, cookie);
    }

    public static IDisposable Register(IServiceProvider serviceProvider, string categoryID)
    {
      var cookie = serviceProvider
        .GetService<ObjectExtenders>()
        .RegisterExtenderProvider(categoryID, EfficioPackage.Name, new Provider(serviceProvider, categoryID));

      return Disposable.Create(() => serviceProvider.GetService<ObjectExtenders>().UnregisterExtenderProvider(cookie));
    }

    private readonly int _cookie;
    private readonly IVsHierarchy _project;
    private readonly ProjectItem _projectItem;
    private readonly uint _projectItemID;
    private readonly IVsBuildPropertyStorage _propertyStorage;
    private readonly IExtenderSite _site;

    /// <summary>
    /// Gets or sets the file extenderName of the template used by the <see cref="CustomTool"/>.
    /// </summary>
    [Category(EfficioPackage.Name)]
    [DisplayName("Custom Tool " + ProjectItemMetadata.Template)]
    [Description("A template used by the " + CustomTool.Name + " to generate code from this file.")]
    [Editor(typeof(CustomToolTemplateEditor), typeof(UITypeEditor))]
    public string CustomToolTemplate
    {
      get
      {
        var value = default(string);

        if (ErrorHandler.Failed(_propertyStorage.GetItemAttribute(_projectItemID, ProjectItemMetadata.Template, out value)))
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

        ErrorHandler.ThrowOnFailure(_propertyStorage.SetItemAttribute(_projectItemID, ProjectItemMetadata.Template, value));

        // If the file does not have a custom tool yet, assume that by specifying the template user wants to use the T4Toolbox.TemplatedFileGenerator.
        if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(customToolValue))
        {
          _projectItem.Properties.Item(ProjectItemProperty.CustomTool).Value = CustomTool.Name;
        }
      }
    }
    
    internal ProjectItemExtender(IServiceProvider serviceProvider, IVsBrowseObject browseObject, IExtenderSite site, int cookie)
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

      var projectItemObject = default(object);

      ErrorHandler.ThrowOnFailure(browseObject.GetProjectItem(out _project, out _projectItemID));
      ErrorHandler.ThrowOnFailure(_project.GetProperty(_projectItemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out projectItemObject));

      _cookie = cookie;
      _projectItem = (ProjectItem) projectItemObject;
      _propertyStorage = (IVsBuildPropertyStorage) _project;
      _site = site;
    }

    ~ProjectItemExtender()
    {
      try
      {
        _site.NotifyDelete(_cookie);
      }
      catch (InvalidOperationException)
      {
      }
    }
  }
}