﻿using System;
using System.Globalization;

using Microsoft.VisualStudio.Shell;

namespace Efficio
{
  /// <summary>
  /// This attribute adds a custom file generator registry entry for specific file 
  /// type. 
  /// For Example:
  ///   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\%Visual Studio Version%\Generators\%Generator GUID%\%Generator Name%]
  ///		"CLSID"="{AAAA53CC-3D4F-40a2-BD4D-4F3419755476}"
  ///   "GeneratesDesignTimeSource" = d'1'
  /// 
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class CodeGeneratorRegistrationAttribute : RegistrationAttribute
  {
    /// <summary>
    /// Creates a new CodeGeneratorRegistrationAttribute attribute to register a custom code generator for the provided context. 
    /// </summary>
    /// <param name="generatorType">The type of Code generator. Type that implements IVsSingleFileGenerator</param>
    /// <param name="generatorName">The generator name</param>
    /// <param name="contextGuid">The context GUID this code generator would appear under.</param>
    public CodeGeneratorRegistrationAttribute(Type generatorType, string generatorName, string contextGuid)
    {
      if (generatorType == null)
      {
        throw new ArgumentNullException(nameof(generatorType));
      }

      if (generatorName == null)
      {
        throw new ArgumentNullException(nameof(generatorName));
      }

      if (contextGuid == null)
      {
        throw new ArgumentNullException(nameof(contextGuid));
      }

      ContextGuid = contextGuid;
      GeneratorType = generatorType;
      GeneratorName = generatorName;
      GeneratorRegKeyName = generatorType.Name;
      GeneratorGuid = generatorType.GUID;
    }

    /// <summary>
    /// Get the generator Type
    /// </summary>
    public Type GeneratorType
    {
      get;
    }

    /// <summary>
    /// Get the Guid representing the project type
    /// </summary>
    public string ContextGuid
    {
      get;
    }

    /// <summary>
    /// Get the Guid representing the generator type
    /// </summary>
    public Guid GeneratorGuid
    {
      get;
    }

    /// <summary>
    /// Get or Set the GeneratesDesignTimeSource value
    /// </summary>
    public bool GeneratesDesignTimeSource
    {
      get;
      set;
    } = false;

    /// <summary>
    /// Get or Set the GeneratesSharedDesignTimeSource value
    /// </summary>
    public bool GeneratesSharedDesignTimeSource
    {
      get;
      set;
    } = false;


    /// <summary>
    /// Gets the Generator name 
    /// </summary>
    public string GeneratorName
    {
      get;
    }

    /// <summary>
    /// Gets the Generator reg key name under 
    /// </summary>
    public string GeneratorRegKeyName
    {
      get;
      set;
    }

    /// <summary>
    /// Property that gets the generator base key name
    /// </summary>
    private string GeneratorRegKey => string.Format(CultureInfo.InvariantCulture, @"Generators\{0}\{1}", ContextGuid, GeneratorRegKeyName);

    /// <summary>
    ///     Called to register this attribute with the given context.  The context
    ///     contains the location where the registration inforomation should be placed.
    ///     It also contains other information such as the type being registered and path information.
    /// </summary>
    public override void Register(RegistrationContext context)
    {
      using (var childKey = context.CreateKey(GeneratorRegKey))
      {
        childKey.SetValue(string.Empty, GeneratorName);
        childKey.SetValue("CLSID", GeneratorGuid.ToString("B"));

        if (GeneratesDesignTimeSource)
        {
          childKey.SetValue("GeneratesDesignTimeSource", 1);
        }

        if (GeneratesSharedDesignTimeSource)
        {
          childKey.SetValue("GeneratesSharedDesignTimeSource", 1);
        }
      }
    }

    /// <summary>
    /// Unregister this file extension.
    /// </summary>
    /// <param name="context"></param>
    public override void Unregister(RegistrationContext context) => context.RemoveKey(GeneratorRegKey);
  }
}
