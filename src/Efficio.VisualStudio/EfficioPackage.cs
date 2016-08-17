using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

using VSLangProj;

namespace Efficio
{
  [Guid("868FDEA8-5E62-4236-91DE-021C837D93C2")]
  [InstalledProductRegistration(Name, Description, "14.0.0.0")]
  [PackageRegistration(UseManagedResourcesOnly = true)]

  // Auto-load the package for C# projects
  [ProvideAutoLoad(VSConstants.UICONTEXT.CSharpProject_string)]

  // Auto-load the package for VB.NET projects
  //[ProvideAutoLoad(VSConstants.UICONTEXT.VBProject_string)]

  // Ensure VS experimental hive can find the extension library.
  [ProvideBindingPath]
  public sealed class EfficioPackage : Package
  {
    public const string Description = "The EfficioPackage extension extends C# and VB.NET project items with addition properties and a new custom tool '" + Name + "'.";
    public const string Name = "EfficioPackage";

    private readonly Stack<IDisposable> _extenders = new Stack<IDisposable>();

    protected override void Initialize()
    {
      base.Initialize();

      _extenders.Push(ProjectItemExtender.Register(this, PrjBrowseObjectCATID.prjCATIDCSharpFileBrowseObject));
      _extenders.Push(ProjectItemExtender.Register(this, PrjBrowseObjectCATID.prjCATIDVBFileBrowseObject));
    }

    protected override void Dispose(bool disposing)
    {
      while (_extenders.Count > 0)
      {
        _extenders.Pop().Dispose();
      }

      base.Dispose(disposing);
    }
  }
}