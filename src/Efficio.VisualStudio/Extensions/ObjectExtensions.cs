namespace System
{
  internal static class ObjectExtensions
  {
    public static T Cast<T>(this object instance, bool safeCast = false) where T : class => (T) instance;

    public static T SafeCast<T>(this object instance) where T : class => instance as T;
  }
}