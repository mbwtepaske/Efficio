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
  public class ScriptFileNameEditor : UITypeEditor
  {
    #region Static

    private static string GetRelativePath(string from, string to)
    {
      var relativePathBuilder = new StringBuilder(260);

      if (TryGetRelativePath(relativePathBuilder, from, to))
      {
        var relativePath = relativePathBuilder.ToString();

        // Remove leading .\ from the path
        if (relativePath.StartsWith("." + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
          relativePath = relativePath.Substring(relativePath.IndexOf(Path.DirectorySeparatorChar) + 1);
        }

        return relativePath;
      }

      throw new ArgumentException($"Cannot find a relative path from '{from}' to '{to}'.");
    }

    private static bool TryGetRelativePath(StringBuilder path, string from, string to) => TryGetRelativePath(path, from, 0, to, 0);

    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="path"> 
    /// A pointer to a string that receives the relative path. This buffer is assumed to be at least MAX_PATH characters in size. 
    /// </param>
    /// <param name="from">
    /// A pointer to a null-terminated string of maximum length MAX_PATH that contains the path that defines the start of the relative path.
    /// </param>
    /// <param name="fromAttributes">
    /// The file attributes of <paramref name="from"/>. 
    /// If this value contains FILE_ATTRIBUTE_DIRECTORY, from is assumed to be a directory; otherwise, from is assumed to be a file.
    /// </param>
    /// <param name="to">
    /// A pointer to a null-terminated string of maximum length MAX_PATH that contains the path that defines the endpoint of the relative path. 
    /// </param>
    /// <param name="toAttributes">
    /// The file attributes of <paramref name="to"/>. 
    /// If this value contains FILE_ATTRIBUTE_DIRECTORY, to is assumed to be directory; otherwise, to is assumed to be a file.
    /// </param>
    /// <returns>
    /// <c>true</c> if method succeeded and <c>false</c> if it failed.
    /// </returns>
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, EntryPoint = "PathRelativePathTo")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TryGetRelativePath([Out] StringBuilder path
      , [In] string from
      , [In] uint fromAttributes
      , [In] string to
      , [In] uint toAttributes);

    #endregion

    /// <summary>
    /// Uses the Windows Open File dialog to allow user to choose the template file.
    /// </summary>
    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      var fileName = value as string ?? String.Empty;
      var projectDirectoryName = Path.GetDirectoryName(Helper.GetProject().FullName);
      var dialog = new OpenFileDialog
      {
        FileName = fileName,
        Filter = "C# Scripts (*.csx)|*.csx|All Files (*.*)|*.*",
        InitialDirectory = projectDirectoryName,
        Title = "Select C# Script"
      };
      
      return dialog.ShowDialog().GetValueOrDefault() ? GetRelativePath(projectDirectoryName, dialog.FileName) : value;
    }

    /// <summary>
    /// Defines the editor as a modal dialog.
    /// </summary>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;
  }
}