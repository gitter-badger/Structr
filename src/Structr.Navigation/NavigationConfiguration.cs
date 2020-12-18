using System;

namespace Structr.Navigation
{
    public class NavigationConfiguration<TNavigationItem> where TNavigationItem : NavigationItem<TNavigationItem>
    {
        public INavigationProvider<TNavigationItem> Provider { get; }
        public NavigationOptions<TNavigationItem> Options { get; }

        public NavigationConfiguration(INavigationProvider<TNavigationItem> provider, NavigationOptions<TNavigationItem> options)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Provider = provider;
            Options = options;
        }
    }
}
