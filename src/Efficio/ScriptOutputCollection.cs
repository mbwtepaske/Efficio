using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Efficio
{
  public class ScriptOutputCollection : ScriptOutput, IEnumerable<ScriptOutput>
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
    
    private readonly Dictionary<string, ScriptOutput> _items = new Dictionary<string, ScriptOutput>(StringComparer.OrdinalIgnoreCase);

    public ScriptOutput this[string filePath]
    {
      get
      {
        if (String.IsNullOrWhiteSpace(filePath))
        {
          return this;
        }

        var scriptOutput = default(ScriptOutput);

        if (!_items.TryGetValue(filePath, out scriptOutput))
        {
          _items[filePath] = scriptOutput = new ScriptOutputItem(this, filePath);
        }
        
        return scriptOutput;
      }
    }
    
    public IEnumerator<ScriptOutput> GetEnumerator()
    {
      yield return this;

      foreach (var scriptOutput in _items.Values)
      {
        yield return scriptOutput;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}