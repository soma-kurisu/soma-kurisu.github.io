using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Services;

public class LazyLoader
{
    public List<Assembly> AdditionalAssemblies { get; } = new();
    private readonly LazyAssemblyLoader _lazyAssemblyLoader;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<LazyLoader> _logger;

    public LazyLoader(
        LazyAssemblyLoader lazyAssemblyLoader,
        NavigationManager navigationManager,
        ILogger<LazyLoader> logger)
    {
        _lazyAssemblyLoader = lazyAssemblyLoader;
        _navigationManager = navigationManager;
        _logger = logger;
    }

    public async Task PreloadAsync()
    {
        var uri = new Uri(_navigationManager.Uri);
        await OnNavigateAsync(uri.LocalPath.Trim('/'));
    }

    public async Task OnNavigateAsync(NavigationContext context) => await OnNavigateAsync(context.Path.Trim('/'));
    public async Task OnNavigateAsync(string path)
    {
        try
        {
            // lazy load component-specific assemblies by name, e.g.:
            if (path == "lazy-component")
            {
                var assemblies = await _lazyAssemblyLoader.LoadAssembliesAsync(new[] { "LazyComponent.dll" });
                AdditionalAssemblies.AddRange(assemblies);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading assemblies");
        }
    }
}