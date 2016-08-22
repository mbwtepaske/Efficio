namespace Efficio
{
  public struct ScriptError
  {
    public readonly string Message;
    public readonly int Column;
    public readonly int Line;

    public ScriptError(int line, int column, string message)
    {
      Line = line;
      Column = column;
      Message = message;
    }
  }
}