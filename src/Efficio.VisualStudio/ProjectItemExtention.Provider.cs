using System;
using System.Reactive.Disposables;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using EnvDTE;

using IExtenderProvider = EnvDTE.IExtenderProvider;

namespace Efficio
{
  /// <summary>
  /// Adds "Custom Tool Script" and "Custom Tool Parameters" properties to the C# and VB.NET project item properties.
  /// </summary>
  public partial class ProjectItemExtension
  {
    private class Provider : IExtenderProvider
    {
      private readonly string _category;
      private readonly IServiceProvider _serviceProvider;

      public Provider(IServiceProvider serviceProvider, string category)
      {
        _category = category;
        _serviceProvider = serviceProvider;
      }

      public bool CanExtend(string category, string extenderName, object extendee) 
        => category == _category && extenderName == EfficioPackage.Name;

      public object GetExtender(string categoryID, string name, object extendee, IExtenderSite extenderSite, int cookie) 
        => new ProjectItemExtension(_serviceProvider, (IVsBrowseObject) extendee, extenderSite, cookie);
    }

    public static IDisposable Register(IServiceProvider serviceProvider, string category)
    {
      var cookie = serviceProvider
        .GetService<ObjectExtenders>()
        .RegisterExtenderProvider(category, EfficioPackage.Name, new Provider(serviceProvider, category));

      return Disposable.Create(() => serviceProvider.GetService<ObjectExtenders>().UnregisterExtenderProvider(cookie));
    }
  }
}