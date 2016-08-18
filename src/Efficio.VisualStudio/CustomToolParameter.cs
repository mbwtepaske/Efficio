using System;
using System.ComponentModel;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Efficio
{
  /// <summary>
  /// Represents a single parameter defined in a text template in the Properties window of Visual Studio.
  /// </summary>
  internal class CustomToolParameter : PropertyDescriptor
  {
    private readonly TypeConverter _converter;
    private readonly string _description;
    private readonly Type _parameterType;

    public override Type ComponentType => typeof(CustomToolParameters);

    public override bool IsReadOnly => !_converter.CanConvertTo(typeof(string)) || !_converter.CanConvertFrom(typeof(string));

    public override Type PropertyType => _parameterType;

    public CustomToolParameter(string parameterName, Type parameterType, string description) : base(parameterName, null)
    {
      _converter = TypeDescriptor.GetConverter(parameterType);
      _description = description;
      _parameterType = parameterType;
    }

    protected override AttributeCollection CreateAttributeCollection() => !string.IsNullOrWhiteSpace(_description) 
      ? AttributeCollection.FromExisting(base.CreateAttributeCollection(), new DescriptionAttribute(_description)) 
      : base.CreateAttributeCollection();

    public override bool CanResetValue(object component) => true;

    private object GetDefaultValue() => _parameterType.IsValueType && _parameterType != typeof(void) 
      ? Activator.CreateInstance(_parameterType) 
      : null;

    private static void GetProjectItem(object component, out IVsBuildPropertyStorage project, out uint itemId)
    {
      if (component == null)
      {
        throw new ArgumentNullException(nameof(component));
      }

      var parent = component as CustomToolParameters;

      if (parent == null)
      {
        throw new ArgumentException($"Object of type {typeof(CustomToolParameters).FullName} is expected, actual object is of type {component.GetType().FullName}.", nameof(component));
      }

      parent.GetProjectItem(out project, out itemId);
    }

    public override object GetValue(object component)
    {
      IVsBuildPropertyStorage project;
      uint itemId;

      GetProjectItem(component, out project, out itemId);

      string stringValue;

      return ErrorHandler.Succeeded(project.GetItemAttribute(itemId, Name, out stringValue))
        ? _converter.ConvertFrom(stringValue)
        : GetDefaultValue();
    }

    public override void ResetValue(object component)
    {
      IVsBuildPropertyStorage project;

      uint itemId;
      GetProjectItem(component, out project, out itemId);

      ErrorHandler.ThrowOnFailure(project.SetItemAttribute(itemId, Name, null));
    }

    public override void SetValue(object component, object value)
    {
      IVsBuildPropertyStorage project;

      uint itemId;
      GetProjectItem(component, out project, out itemId);

      if (Equals(value, GetDefaultValue()))
      {
        ErrorHandler.ThrowOnFailure(project.SetItemAttribute(itemId, Name, null));
      }
      else
      {
        var stringValue = _converter.ConvertToInvariantString(value);

        ErrorHandler.ThrowOnFailure(project.SetItemAttribute(itemId, Name, stringValue));
      }
    }

    /// <summary>
    /// Returns true when property value is different than the default. 
    /// </summary>
    /// <remarks>
    /// This is used by the PropertyGrid in Visual Studio to display values that are actually stored in bold font.
    /// </remarks>
    public override bool ShouldSerializeValue(object component) => !Equals(GetValue(component), GetDefaultValue());
  }
}