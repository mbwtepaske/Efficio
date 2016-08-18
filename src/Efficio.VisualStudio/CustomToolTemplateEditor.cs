using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace Efficio
{
  /// <summary>
  /// A specialized file name editor for the template property.
  /// </summary>
  public class CustomToolTemplateEditor : UITypeEditor
  {
    /// <summary>
    /// Uses the Windows Open File dialog to allow user to choose the template file.
    /// </summary>
    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      var dialog = new OpenFileDialog
      {
        InitialDirectory = Path.GetDirectoryName(Helper.GetProject().FullName),
        //FileName = Path.GetFileName(templateFullPath),
        Filter = "C# Scripts (*.csx)|*.csx|VB Scripts (*.vbx)|*.vbx|All Files (*.*)|*.*",
        Title = "Select Custom Tool Template"
      };

      return dialog.ShowDialog() == true 
        ? GetRelativeTemplatePath(context, dialog.FileName) 
        : value;
    }

    /// <summary>
    /// Defines the editor as a modal dialog.
    /// </summary>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;

    private static string GetFullInputPath(ITypeDescriptorContext context)
    {
      var projectItem = (IVsBrowseObject) context.Instance;
      var project = default(IVsHierarchy);
      var projectItemID = 0U;

      ErrorHandler.ThrowOnFailure(projectItem.GetProjectItem(out project, out projectItemID));

      var inputFileName = default(string);

      ErrorHandler.ThrowOnFailure(project.GetCanonicalName(projectItemID, out inputFileName));

      return inputFileName;
    }

    //private static string GetFullTemplatePath(ITypeDescriptorContext context, string fileName)
    //{
    //  var inputFullPath = GetFullInputPath(context);

    //  if (string.IsNullOrEmpty(fileName))
    //  {
    //    return Path.GetDirectoryName(inputFullPath) + Path.DirectorySeparatorChar;
    //  }

    //  var templateFullPath = fileName;
    //  var templateLocator = (TemplateLocator)context.GetService(typeof(TemplateLocator));

    //  return !templateLocator.LocateTemplate(inputFullPath, ref templateFullPath) 
    //    ? Path.Combine(Path.GetDirectoryName(inputFullPath), fileName) 
    //    : templateFullPath;
    //}

    private static string GetRelativeTemplatePath(ITypeDescriptorContext context, string fullPath)
    {
      var inputPath = GetFullInputPath(context);
      var relativePathBuilder = new StringBuilder(260);

      if (!PathRelativePathTo(relativePathBuilder, inputPath, 0, fullPath, 0))
      {
        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, $"Cannot convert '{inputPath}' to a path relative to the location of '{fullPath}'."));
      }

      var relativePath = relativePathBuilder.ToString();

      if (relativePath.StartsWith("." + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
      {
        // Remove leading .\ from the path
        relativePath = relativePath.Substring(relativePath.IndexOf(Path.DirectorySeparatorChar) + 1);
      }

      return relativePath;
    }

    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="path"> 
    /// A pointer to a string that receives the relative path. This buffer is 
    /// assumed to be at least MAX_PATH characters in size. 
    /// </param>
    /// <param name="from">
    /// A pointer to a null-terminated string of maximum length MAX_PATH that 
    /// contains the path that defines the start of the relative path.
    /// </param>
    /// <param name="fromAttributes">
    /// The file attributes of <paramref name="from"/>. If this value contains FILE_ATTRIBUTE_DIRECTORY, 
    /// from is assumed to be a directory; otherwise, from is assumed to be a file.
    /// </param>
    /// <param name="to">
    /// A pointer to a null-terminated string of maximum length MAX_PATH that contains 
    /// the path that defines the endpoint of the relative path. 
    /// </param>
    /// <param name="toAttributes">
    /// The file attributes of <paramref name="to"/>. If this value contains FILE_ATTRIBUTE_DIRECTORY, 
    /// to is assumed to be directory; otherwise, to is assumed to be a file.
    /// </param>
    /// <returns>
    /// <c>true</c> if method succeeded and <c>false</c> if it failed.
    /// </returns>
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool PathRelativePathTo([Out] StringBuilder path, [In] string from, [In] uint fromAttributes, [In] string to, [In] uint toAttributes);
  }
}