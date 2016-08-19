using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Efficio
{
  public class Script<TContent> : IDisposable 
    where TContent : class
  {
    public static async Task<ScriptResult> EvaluateAsync(string scriptCode, string scriptPath = null, TContent content = null)
    {
      var scriptOptions = ScriptOptions.Default
        .WithFilePath(scriptPath)
        .WithReferences(typeof(Script).Assembly)
        .WithImports(typeof(Script).Namespace);

      using (var script = new Script<TContent>(content))
      {
        var scriptResult = new ScriptResult();

        try
        {
          await CSharpScript.EvaluateAsync(scriptCode ?? String.Empty, scriptOptions, script);
        }
        catch (AggregateException)
        {
        }
        catch (CompilationErrorException exception)
        {
          
        }

        return scriptResult;
      }
    }

    public TContent Content
    {
      get;
      set;
    }

    private Script(TContent content = null)
    {
      Content = content;
    }

    public void Dispose()
    {
    }
  }
}