namespace Efficio
{
  /// <summary>
  /// Defines constants for commonly-used VisualStudio project item properties.
  /// </summary>
  internal static class ProjectItemProperty
  {
    /// <summary>
    /// Internal name of the "Copy to Output Directory" project item property.
    /// </summary>
    public const string CopyToOutputDirectory = "CopyToOutputDirectory";

    /// <summary>
    /// Internal name of the "Custom Tool" project item property.
    /// </summary>
    public const string CustomTool = "EfficioGenerator";

    /// <summary>
    /// Internal name of the "Custom Tool Namespace" project item property.
    /// </summary>
    public const string CustomToolNamespace = "CustomToolNamespace";

    /// <summary>
    /// Internal name of the "Custom Tool Parameters" project item property provided by the T4 Toolbox.
    /// </summary>
    public const string CustomToolParameters = EfficioPackage.Name + ".CustomToolParameters";

    /// <summary>
    /// Internal name of the "Custom Tool Script" project item property provided by the T4 Toolbox.
    /// </summary>
    public const string CustomToolTemplate = EfficioPackage.Name + ".Script";

    /// <summary>
    /// Internal name of the "Build Action" project item property.
    /// </summary>
    public const string ItemType = "ItemType";
  }
}