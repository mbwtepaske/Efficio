using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Efficio
{
  public class Script : IDisposable 
  {
    public static async Task<ScriptResult> EvaluateAsync(string scriptCode, string scriptPath = null, string content = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      var scriptOptions = ScriptOptions.Default
        .WithFilePath(scriptPath)
        .WithReferences(typeof(Script).Assembly)
        .WithImports(typeof(Script).Namespace);

      using (var script = new Script(content))
      {
        try
        {
          await CSharpScript.EvaluateAsync(scriptCode ?? String.Empty, scriptOptions, script, script.GetType(), cancellationToken);
        }
        catch (CompilationErrorException exception)
        {
          var errorList = new List<ScriptError>();

          foreach (var diagnostic in exception.Diagnostics)
          {
            var position = diagnostic.Location.GetLineSpan().StartLinePosition;
            
            errorList.Add(new ScriptError(position.Line, position.Character, diagnostic.GetMessage()));
          }
          
          return new ScriptResult(errorList.ToArray());
        }

        return new ScriptResult(script.Output.ToArray());
      }
    }

    public string Content
    {
      get;
      set;
    }

    public ScriptOutputCollection Output
    {
      get;
    } = new ScriptOutputCollection();

    public Script(string content)
    {
      Content = content ?? String.Empty;
    }

    public void Dispose() => Output.Dispose();
  }
}