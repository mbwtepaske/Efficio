using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;

using Microsoft.Win32;

namespace Efficio
{
  /// <summary>
  /// A specialized file name editor for the template property.
  /// </summary>
  public class ScriptFileNameEditor : UITypeEditor
  {
    private static string GetRelativePath(string directoryName, string fileName)
    {
      if (!directoryName.EndsWith(Path.DirectorySeparatorChar.ToString()))
      {
        directoryName += Path.DirectorySeparatorChar;
      }
      
      return Uri.UnescapeDataString(new Uri(directoryName).MakeRelativeUri(new Uri(fileName)).ToString().Replace('/', Path.DirectorySeparatorChar));
    }

    /// <summary>
    /// Uses the Windows Open File dialog to allow user fileName choose the template file.
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