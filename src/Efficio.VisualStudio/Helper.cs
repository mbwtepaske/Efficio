using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Efficio
{
  internal static class Helper
  {
    public static IEnumerable<Project> GetActiveProjects()
    {
      var activeSolutionProjects = ServiceProvider.GlobalProvider.GetService<SDTE, DTE>().ActiveSolutionProjects as Array;

      if (activeSolutionProjects != null)
      {
        for (var index = 0; index < activeSolutionProjects.Length; index++)
        {
          yield return (Project) activeSolutionProjects.GetValue(index);
        }
      }
    }

    public static string GetFullPath(ProjectItem projectItem) => Convert.ToString(projectItem.Properties.Item("FullPath").Value);

    public static ProjectItem GetProjectItem(this IVsHierarchy hierarchy, uint itemID)
    {
      var projectItem = default(object);

      ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(itemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out projectItem));

      return projectItem as ProjectItem;
    }

    private static Project GetProject(this IVsHierarchy hierarchy, uint itemID)
    {
      var projectItem = default(object);

      ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(itemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out projectItem));

      return projectItem is ProjectItem 
        ? ((ProjectItem) projectItem).ContainingProject 
        : (Project) projectItem;
    }

    public static Project GetProjectFromExplorer(DTE2 dte)
    {
      var hierarchy = dte.ToolWindows.SolutionExplorer;

      var selectedItems = (Array)hierarchy.SelectedItems;

      if (selectedItems != null && selectedItems.Length > 0)
      {
        foreach (UIHierarchyItem selectedItem in selectedItems)
        {
          var projectItem = selectedItem.Object as ProjectItem;

          if (projectItem?.ContainingProject != null)
          {
            return projectItem.ContainingProject;
          }
        }
      }

      return null;
    }
    
    public static string GetProjectFolder(Project project) => Path.GetDirectoryName(project?.FullName);

    public static Project GetProject()
    {
      var projectItemID = 0U;

      return GetProject(GetCurrentHierarchy(out projectItemID), projectItemID);
    }

    public static IVsHierarchy GetCurrentHierarchy(out uint projectItemId)
    {
      var hierarchyPointer = IntPtr.Zero;
      var selectionContainerPointer = IntPtr.Zero;
      var multiItemSelection = default(IVsMultiItemSelect);

      ServiceProvider.GlobalProvider.GetService<SVsShellMonitorSelection, IVsMonitorSelection>().GetCurrentSelection(out hierarchyPointer, out projectItemId, out multiItemSelection, out selectionContainerPointer);

      return Marshal.GetTypedObjectForIUnknown(hierarchyPointer, typeof(IVsHierarchy)) as IVsHierarchy;
    }

    public static DTE2 GetDte2() => (DTE2) ServiceProvider.GlobalProvider.GetService<DTE>();

    public static string GetSourceFilePath()
    {
      foreach (UIHierarchyItem selectionItem in GetDte2().ToolWindows.SolutionExplorer.SelectedItems.SafeCast<Array>() ?? Array.Empty<UIHierarchyItem>())
      {
        return selectionItem.Object.SafeCast<ProjectItem>()?.Properties.Item("FullPath").Value.ToString() ?? string.Empty;
      }

      return string.Empty;
    }

    public static bool IsItemInsideFolder(string itemPath, string folderPath) => itemPath.StartsWith(folderPath, true, CultureInfo.InvariantCulture);
  }
}
