using System;
using System.Collections.Generic;

namespace Efficio
{
  public class ScriptResult
  {
    public IReadOnlyList<ScriptError> Errors
    {
      get;
    }

    public IReadOnlyList<ScriptOutput> Outputs
    {
      get;
    }

    internal ScriptResult(IReadOnlyList<ScriptError> errors)
    {
      Errors = errors;
    }

    internal ScriptResult(IReadOnlyList<ScriptOutput> outputs, IReadOnlyList<ScriptError> errors = null) : this(errors ?? Array.Empty<ScriptError>())
    {
      Outputs = outputs;
    }
  }
}