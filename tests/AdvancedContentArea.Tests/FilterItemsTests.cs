using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TechFellow.Optimizely.AdvancedContentArea.Tests;

public class FilterItemsTests
{
    [Fact]
    public void Filters_ChangeTheListInPlace()
    {
        // FilterAsync returns a plain ValueTask - filters remove items from the list they are given
        var services = CreateServices(items =>
        {
            items.RemoveAt(1);
            return ValueTask.CompletedTask;
        });

        var result = AdvancedContentAreaRenderer.FilterItems(services, CreateArea(1, 2, 3));

        Assert.Equal([1, 3], result.Select(i => i.ContentLink.ID));
    }

    [Fact]
    public void SynchronouslyFaultedFilter_IsNotSwallowed()
    {
        var services = CreateServices(_ => ValueTask.FromException(new InvalidOperationException("filter failed")));

        Assert.Throws<InvalidOperationException>(() => AdvancedContentAreaRenderer.FilterItems(services, CreateArea(1)));
    }

    private static ContentArea CreateArea(params int[] ids)
    {
        var area = new ContentArea();
        foreach (var id in ids)
        {
            area.Items.Add(new ContentAreaItem { ContentLink = new ContentReference(id) });
        }

        return area;
    }

    private static IServiceProvider CreateServices(Func<IList<ContentAreaItem>, ValueTask> filter) =>
        new ServiceCollection()
            .AddSingleton<IPrincipalAccessor, PrincipalAccessor>()
            .AddSingleton<IContextModeResolver, ContextModeResolver>()
            .AddSingleton<IContentAreaItemsRenderingFilter>(new DelegateFilter(filter))
            .BuildServiceProvider();

    private sealed class DelegateFilter(Func<IList<ContentAreaItem>, ValueTask> filter) : IContentAreaItemsRenderingFilter
    {
        public ValueTask FilterAsync(IList<ContentAreaItem> items, IPrincipal principal, ContextMode mode) => filter(items);
    }

    private sealed class PrincipalAccessor : IPrincipalAccessor
    {
        public IPrincipal Principal { get; set; }
    }

    private sealed class ContextModeResolver : IContextModeResolver
    {
        public ContextMode CurrentMode => ContextMode.Default;
    }
}
