namespace System
{
  public static class ServiceProviderExtensions
  {
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    public static TService GetService<TService>(this IServiceProvider serviceProvider) => (TService) serviceProvider.GetService(typeof(TService));

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    public static T GetService<TService, T>(this IServiceProvider serviceProvider) => (T) serviceProvider.GetService(typeof(TService));
  }
}