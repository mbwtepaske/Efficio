using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Efficio
{
  /// <summary>
  /// Represents a collection of text template parameters in the Properties window of Visual Studio.
  /// </summary>
  /// <remarks>
  /// This class implements the <see cref="ICustomTypeDescriptor"/> interface to present template parameters
  /// as a dynamic list of <see cref="PropertyDescriptor"/> objects.
  /// </remarks>
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class CustomToolParameters : ICustomTypeDescriptor
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IVsHierarchy _project;
    private readonly uint _projectItemID;
    private string[] _assemblyReferences;

    internal CustomToolParameters(IServiceProvider serviceProvider, IVsHierarchy project, uint projectItemId)
    {
      _serviceProvider = serviceProvider;
      _project = project;
      _projectItemID = projectItemId;
    }

    #region ICustomTypeConverter

    /// <summary>
    /// Returns a default collection of attributes applied to this class.
    /// </summary>
    public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this, true);

    /// <summary>
    /// Returns the default name of this class.
    /// </summary>
    public string GetClassName() => TypeDescriptor.GetClassName(this, true);

    /// <summary>
    /// Returns the default name of this component.
    /// </summary>
    public string GetComponentName() => TypeDescriptor.GetComponentName(this, true);

    /// <summary>
    /// Returns a <see cref="TypeConverter"/> specified in a <see cref="TypeConverterAttribute"/> applied to this class.
    /// </summary>
    public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this, true);

    /// <summary>
    /// Returns a default event defined in this class.
    /// </summary>
    public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);

    /// <summary>
    /// Returns a default property defined in this class.
    /// </summary>
    public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this, true);

    /// <summary>
    /// Returns a default editor of this class.
    /// </summary>
    public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);

    /// <summary>
    /// Returns a default collection of events with the given attributes defined in this class.
    /// </summary>
    public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this, attributes, true);

    /// <summary>
    /// Returns a default collection of events defined in this class.
    /// </summary>
    public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(this, true);

    /// <summary>
    /// Returns a collection of <see cref="CustomToolParameter"/> objects representing parameters defined in a text template.
    /// </summary>
    public PropertyDescriptorCollection GetProperties() => GetProperties(null);

    /// <summary>
    /// Returns a collection of <see cref="CustomToolParameter"/> objects representing parameters defined in a text template.
    /// </summary>
    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
      string templateFileName;

      if (ResolveTemplate(out templateFileName))
      {
        //string templateContent = File.ReadAllText(templateFileName, EncodingHelper.GetEncoding(templateFileName));

        for (int i = 0; i < _assemblyReferences.Length; i++)
        {
        //  _assemblyReferences[i] = this.templatingHost.ResolveAssemblyReference(_assemblyReferences[i]);
        }

        var parameters = new List<CustomToolParameter>();

        //ParseParameters(templateContent, parameters);

        return new PropertyDescriptorCollection(parameters.Cast<PropertyDescriptor>().ToArray());
      }

      return PropertyDescriptorCollection.Empty;
    }

    /// <summary>
    /// Returns an owner of the specified <see cref="PropertyDescriptor"/>.
    /// </summary>
    public object GetPropertyOwner(PropertyDescriptor propertyDescriptor) => this;

    #endregion

    /// <summary>
    /// Returns an empty string to prevent the type name from being displayed in Visual Studio Properties window.
    /// </summary>
    public override string ToString() => string.Empty;

    internal void GetProjectItem(out IVsBuildPropertyStorage project, out uint projectItemID)
    {
      project = (IVsBuildPropertyStorage) _project;
      projectItemID = _projectItemID;
    }

    private void ParseParameters(string templateContent, List<CustomToolParameter> parameters)
    {
      // Parse any <#@ include #> directives from the template
      //foreach (Match includeMatch in IncludeExpression.Matches(templateContent))
      //{
      //}

      // Parse any <#@ parameter #> directives from the template
      //foreach (Match parameterMatch in ParameterExpression.Matches(templateContent))
      //{
      //  parameters.Add(CreateParameter(parameterMatch));
      //}
    }

    private bool ResolveTemplate(out string templateFileName)
    {
      templateFileName = string.Empty;

      string inputFileName;
      ErrorHandler.ThrowOnFailure(_project.GetCanonicalName(_projectItemID, out inputFileName));

      var propertyStorage = (IVsBuildPropertyStorage)_project;

      string generator;
      if (ErrorHandler.Failed(propertyStorage.GetItemAttribute(_projectItemID, CustomTool.Name, out generator)))
      {
        return false;
      }

      //if (string.Equals(generator, "TextTemplatingFileGenerator", StringComparison.OrdinalIgnoreCase))
      //{
      //  templateFileName = inputFileName;
      //}
      //else if (string.Equals(generator, ScriptFileGenerator.Name, StringComparison.OrdinalIgnoreCase))
      //{
      //  if (ErrorHandler.Failed(propertyStorage.GetItemAttribute(_projectItemID, ItemMetadata.LastGenOutput, out templateFileName)))
      //  {
      //    return false;
      //  }

      //  templateFileName = Path.Combine(Path.GetDirectoryName(inputFileName), templateFileName);
      //}
      //else if (string.Equals(generator, TemplatedFileGenerator.Name, StringComparison.OrdinalIgnoreCase))
      //{
      //  if (ErrorHandler.Failed(propertyStorage.GetItemAttribute(_projectItemID, ItemMetadata.Template, out templateFileName)))
      //  {
      //    return false;
      //  }

      //  var templateLocator = (TemplateLocator)_serviceProvider.GetService(typeof(TemplateLocator));
      //  if (!templateLocator.LocateTemplate(inputFileName, ref templateFileName))
      //  {
      //    return false;
      //  }
      //}

      return File.Exists(templateFileName);
    }

    //private CustomToolParameter CreateParameter(Match match)
    //{
    //  // Resolve parameter type
    //  //var typeName = match.Groups[TypeGroup].Value;
    //  var description = string.Empty;
    //  Type type = null;
    //
    //  try
    //  {
    //  //  type = Type.GetType(typeName, throwOnError: true, assemblyResolver: null, typeResolver: ResolveType);
    //  }
    //  catch (TypeLoadException e)
    //  {
    //    type = typeof(void);
    //    description = e.Message;
    //  }
    //
    //  return new CustomToolParameter(match.Groups[NameGroup].Value, type, description);
    //}

    [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "That's how the T4 Engine loads assemblies.")]
    private Type ResolveType(Assembly assembly, string typeName, bool ignoreCase)
    {
      // Try among assemblies already loaded in the current AppDomain
      foreach (var loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        var type = loadedAssembly.GetType(typeName, false, ignoreCase);

        if (type != null)
        {
          return type;
        }
      }

      // Try among assemblies referenced by the template
      foreach (string assemblyFileName in _assemblyReferences)
      {
        var referencedAssembly = Assembly.LoadFrom(assemblyFileName);
        var type = referencedAssembly.GetType(typeName, false, ignoreCase);

        if (type != null)
        {
          return type;
        }
      }

      return null;
    }
  }
}