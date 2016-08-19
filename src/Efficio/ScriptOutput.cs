using System;
using System.IO;
using System.Text;

namespace Efficio
{
  public class ScriptOutput : TextWriter
  {
    public bool IsDefault => String.IsNullOrWhiteSpace(FilePath);

    public override Encoding Encoding => Encoding.UTF8;

    public string FilePath
    {
      get;
    }

    protected ScriptOutput()
    {
      FilePath = String.Empty;
    }

    protected ScriptOutput(string filePath)
    {
      FilePath = Path.GetFullPath(filePath);
    }
  }
}