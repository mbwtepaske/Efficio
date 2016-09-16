
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio.Designer.Interfaces;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;

namespace Efficio
{
  /// <summary>
  /// Base code generator with site implementation
  /// </summary>
  public abstract class BaseCodeGeneratorWithSite : BaseCodeGenerator, VSOLE.IObjectWithSite
  {
    private CodeDomProvider _codeDomProvider;

    #region IObjectWithSite Members

    private object Site
    {
      get;
      set;
    }

    void VSOLE.IObjectWithSite.GetSite(ref Guid interfaceGuid, out IntPtr interfacePointer)
    {
      if (Site == null)
      {
        throw new COMException("object is not sited", VSConstants.E_FAIL);
      }

      ErrorHandler.ThrowOnFailure(Marshal.QueryInterface(Marshal.GetIUnknownForObject(Site), ref interfaceGuid, out interfacePointer));
    }

    /// <summary>
    /// SetSite method of IOleObjectWithSite
    /// </summary>
    void VSOLE.IObjectWithSite.SetSite(object site)
    {
      Site = site;
      _codeDomProvider = null;
      _serviceProvider = null;
    }

    #endregion

    private ServiceProvider _serviceProvider;

    /// <summary>
    /// Gets the ServiceProvider
    /// </summary>
    protected ServiceProvider ServiceProvider => _serviceProvider ?? (_serviceProvider = new ServiceProvider(Site as VSOLE.IServiceProvider));
    
    /// <summary>
    /// Returns a CodeDomProvider object for the language of the project containing
    /// the project item the generator was called on
    /// </summary>
    /// <returns>A CodeDomProvider object</returns>
    protected virtual CodeDomProvider GetCodeProvider()
    {
      if (_codeDomProvider == null)
      {
        //Query for IVSMDCodeDomProvider/SVSMDCodeDomProvider for this project type
        var provider = ServiceProvider.GetService<SVSMDCodeDomProvider, IVSMDCodeDomProvider>();

        if (provider != null)
        {
          _codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
        }
        else
        {
          //In the case where no language specific CodeDom is available, fall back to C#
          _codeDomProvider = CodeDomProvider.CreateProvider("C#");
        }
      }

      return _codeDomProvider;
    }

    /// <summary>
    /// Gets the default extension of the output file from the CodeDomProvider
    /// </summary>
    protected override string GetDefaultExtension()
    {
      var codeProvider = GetCodeProvider();
      var extension = codeProvider.FileExtension;

      if (!string.IsNullOrEmpty(extension))
      {
        extension = ".generated." + extension.TrimStart(".".ToCharArray());
      }

      return extension;
    }

    /// <summary>
    /// Returns the EnvDTE.Project object of the project containing the project item the code generator was called on.
    /// </summary>
    protected Project GetProject() => GetProjectItem().ContainingProject;

    /// <summary>
    /// Returns the EnvDTE.ProjectItem object that corresponds to the project item the code generator was called on.
    /// </summary>
    protected ProjectItem GetProjectItem() => ServiceProvider.GetService<ProjectItem>();
  }
}
