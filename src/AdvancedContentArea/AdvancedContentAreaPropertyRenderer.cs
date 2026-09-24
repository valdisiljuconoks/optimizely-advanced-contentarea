// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Mvc.TagHelpers;
using EPiServer.Web.Mvc.TagHelpers.Internal;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using TechFellow.Optimizely.AdvancedContentArea.Initialization;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Renders content areas written with the CMS 13 <c>epi-property</c> tag helper through
/// <see cref="AdvancedContentAreaRenderer"/>.
/// <para>
/// CMS 13 routes tag helper properties through <c>IPropertyRenderer</c> instead of <c>ContentAreaRenderer</c>,
/// which means display options, row support and the item callbacks would be lost in tag helper views. This
/// renderer puts them back, so <c>@Html.PropertyFor(x =&gt; x.Area)</c> and
/// <c>&lt;div epi-property="@Model.Area"&gt;&lt;/div&gt;</c> produce the same markup.
/// </para>
/// </summary>
[PropertyRenderer(typeof(ContentArea), SortOrder)]
public class AdvancedContentAreaPropertyRenderer : IPropertyRenderer
{
    /// <summary>
    /// Sort order of this renderer. The built-in content area renderer uses 0 and the highest order wins,
    /// so anything above 0 takes over. Two renderers for the same property type may not share an order.
    /// </summary>
    public const uint SortOrder = 100;

    // keys the CMS tag helpers use to hand the current item over to an `epi-property-item` child template
    private const string ContentAreaTagKey = "EP:TagHelper:ContentAreaTag";
    private const string ContentAreaItemKey = "EP:TagHelper:ContentAreaItem";

    private static readonly string[] FlagAttributes = { "rowsupport", "hasitemcontainer" };

    private static readonly string[] StringAttributes = { "rowcssclass", "childrencssclass", "childrencustomtagname" };

    /// <summary>
    /// Renders the content area into the element carrying the <c>epi-property</c> attribute.
    /// </summary>
    /// <param name="propertyExpression">The property being rendered</param>
    /// <param name="output">Output for the element the tag helper is attached to</param>
    /// <param name="propertyRenderingContext">Current rendering context</param>
    public async Task RenderAsync(
        ModelExpression propertyExpression,
        TagHelperOutput output,
        PropertyRenderingContext propertyRenderingContext)
    {
        var contentArea = propertyExpression.Model as ContentArea;

        if (contentArea == null || contentArea.Count == 0)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(HtmlString.Empty);
            return;
        }

        // a hand written `epi-property-item` child template means the view renders the items itself
        // (that is how the stock CMS 13 views are written) - leave those alone
        var itemTemplate = await output.GetChildContentAsync();

        output.TagMode = TagMode.StartTagAndEndTag;

        if (!itemTemplate.IsEmptyOrWhiteSpace)
        {
            await RenderItemTemplateAsync(contentArea, output, propertyRenderingContext);
            return;
        }

        Render(contentArea, output, propertyRenderingContext);
    }

    private static void Render(ContentArea contentArea, TagHelperOutput output, PropertyRenderingContext context)
    {
        var viewContext = context.ViewContext;
        var services = viewContext.HttpContext.RequestServices;
        var renderer = services.GetRequiredService<ContentAreaRenderer>();

        // the element the tag helper sits on is the wrapping element - the renderer must not add another one
        var viewData = new ViewDataDictionary(viewContext.ViewData) { ["hascontainer"] = false };

        if (!string.IsNullOrEmpty(context.TemplateTag))
        {
            viewData["tag"] = context.TemplateTag;
        }

        MoveRenderSettingsToViewData(output, viewData);

        var options = services.GetService<AdvancedContentAreaRendererOptions>();
        if (options != null && options.AutoAddRow)
        {
            output.AddClass("row", HtmlEncoder.Default);
        }

        var originalViewData = viewContext.ViewData;
        var originalWriter = viewContext.Writer;
        var writer = new HtmlStringWriter();

        try
        {
            viewContext.ViewData = viewData;
            viewContext.Writer = writer;

            var htmlHelper = services.GetRequiredService<IHtmlHelper>();
            ((IViewContextAware)htmlHelper).Contextualize(viewContext);

            renderer.Render(htmlHelper, contentArea);
        }
        finally
        {
            viewContext.ViewData = originalViewData;
            viewContext.Writer = originalWriter;
        }

        output.Content.SetHtmlContent(writer.ToString());
    }

    private static async Task RenderItemTemplateAsync(
        ContentArea contentArea,
        TagHelperOutput output,
        PropertyRenderingContext context)
    {
        var items = AdvancedContentAreaRenderer.FilterItems(context.ViewContext.HttpContext.RequestServices, contentArea);

        context.TagHelperContext.Items[ContentAreaTagKey] = context.TemplateTag;

        foreach (var item in items)
        {
            context.TagHelperContext.Items[ContentAreaItemKey] = item;

            var itemContent = await output.GetChildContentAsync(false);
            if (!itemContent.IsEmptyOrWhiteSpace)
            {
                output.Content.AppendHtml(itemContent);
            }
        }
    }

    /// <summary>
    /// Moves the render settings written as plain attributes (<c>rowsupport</c>, <c>rowcssclass</c>, ..) into
    /// view data, where the renderer reads them, and off the element so they don't end up in the markup.
    /// </summary>
    private static void MoveRenderSettingsToViewData(TagHelperOutput output, ViewDataDictionary viewData)
    {
        foreach (var name in FlagAttributes)
        {
            if (TryTakeAttribute(output, name, out var value))
            {
                // a minimized attribute (`<div epi-property="@x" rowsupport>`) means "on"
                viewData[name] = string.IsNullOrEmpty(value) || !bool.TryParse(value, out var flag) || flag;
            }
        }

        foreach (var name in StringAttributes)
        {
            if (TryTakeAttribute(output, name, out var value))
            {
                viewData[name] = value;
            }
        }
    }

    private static bool TryTakeAttribute(TagHelperOutput output, string name, out string value)
    {
        if (!output.Attributes.TryGetAttribute(name, out var attribute))
        {
            value = null;
            return false;
        }

        value = attribute.Value?.ToString();
        output.Attributes.Remove(attribute);

        return true;
    }
}
