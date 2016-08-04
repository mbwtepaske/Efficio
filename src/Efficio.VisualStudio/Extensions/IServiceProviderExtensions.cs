namespace System
{
  public static class IServiceProviderExtensions
  {
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    public static TService GetService<TService>(this IServiceProvider serviceProvider) => (TService) serviceProvider.GetService(typeof(TService));
  }
}