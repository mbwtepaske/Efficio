using System;
using System.Collections.Generic;

namespace Efficio
{
  public class ScriptOutputCollection : ScriptOutput
  {
    private class ScriptOutputItem : ScriptOutput
    {
      public ScriptOutputCollection Parent
      {
        get;
      }

      public ScriptOutputItem(ScriptOutputCollection parent, string fileName) : base(fileName)
      {
        Parent = parent;
      }
    }
    
    private readonly HashSet<ScriptOutput> _items = new HashSet<ScriptOutput>();

    public ScriptOutput this[string filePath]
    {
      get
      {
        if (String.IsNullOrWhiteSpace(filePath))
        {
          return this;
        }
        
        return new ScriptOutputItem(this, filePath);
      }
    }

    public ScriptOutputCollection()
    {
    }
  }
}