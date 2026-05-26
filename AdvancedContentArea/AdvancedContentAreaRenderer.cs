// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechFellow.Optimizely.AdvancedContentArea.Extensions;
using TechFellow.Optimizely.AdvancedContentArea.Initialization;
using TechFellow.Optimizely.AdvancedContentArea.Providers;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Provides rendering functionality for content areas with extended features and customization options.
/// </summary>
/// <remarks>
/// This class extends the default ContentAreaRenderer to provide advanced rendering capabilities, such as
/// custom CSS class handling, template tagging, and support for display mode fallbacks. It allows for the
/// customization of rendering behavior and content area structure.
/// </remarks>
public class AdvancedContentAreaRenderer : ContentAreaRenderer
{
    private ContentAreaItem _currentContent;
    private Action<HtmlNode, ContentAreaItem, IContentData> _elementStartTagRenderCallback;
    private IEnumerable<DisplayModeFallback> _fallbacks;
    private readonly IContentAreaLoader _contentAreaLoader;
    internal readonly AdvancedContentAreaRendererOptions Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdvancedContentAreaRenderer"/> class.
    /// </summary>
    /// <param name="contentAreaLoader">Loads content referenced by content area items.</param>
    /// <param name="fallbacks">Bootstrap display mode fallbacks used for layout and CSS classes.</param>
    /// <param name="options">Renderer configuration options.</param>
    public AdvancedContentAreaRenderer(
        IContentAreaLoader contentAreaLoader,
        IReadOnlyCollection<DisplayModeFallback> fallbacks, AdvancedContentAreaRendererOptions options)
    {
        _fallbacks = fallbacks ?? throw new ArgumentNullException(nameof(fallbacks));
        _contentAreaLoader = contentAreaLoader ?? throw new ArgumentNullException(nameof(contentAreaLoader));
        Options = options;
    }

    /// <summary>Tag of the content area being rendered, taken from view data when not set on the model.</summary>
    public string ContentAreaTag { get; private set; }

    /// <summary>Default display option for the current content area property, if configured.</summary>
    public string DefaultContentAreaDisplayOption { get; private set; }

    internal void SetElementStartTagRenderCallback(Action<HtmlNode, ContentAreaItem, IContentData> callback)
    {
        _elementStartTagRenderCallback = callback;
    }

    internal void SetDisplayOptions(List<DisplayModeFallback> displayOptions)
    {
        _fallbacks = displayOptions;
    }

    /// <inheritdoc />
    public override void Render(IHtmlHelper htmlHelper, ContentArea contentArea)
    {
        if (contentArea == null || contentArea.IsEmpty)
        {
            return;
        }

        // capture given CA tag (should be contentArea.Tag, but EPiServer is not filling that property)
        ContentAreaTag = htmlHelper.ViewData["tag"] as string;
        if (htmlHelper.ViewData.ModelMetadata.AdditionalValues.ContainsKey(
                $"{nameof(DefaultDisplayOptionMetadataProvider)}__DefaultDisplayOption"))
        {
            DefaultContentAreaDisplayOption =
                htmlHelper.ViewData.ModelMetadata.AdditionalValues[
                    $"{nameof(DefaultDisplayOptionMetadataProvider)}__DefaultDisplayOption"] as string;
        }

        var viewContext = htmlHelper.ViewContext;
        TagBuilder tagBuilder = null;

        if (!IsInEditMode() && ShouldRenderWrappingElement(htmlHelper))
        {
            tagBuilder = new TagBuilder(GetContentAreaHtmlTag(htmlHelper, contentArea));
            AddNonEmptyCssClass(tagBuilder, viewContext.ViewData["cssclass"] as string);

            if (Options.AutoAddRow)
            {
                AddNonEmptyCssClass(tagBuilder, "row");
            }

            viewContext.Writer.Write(tagBuilder.RenderStartTag());
        }

        RenderContentAreaItems(htmlHelper, contentArea.Items);

        if (tagBuilder == null)
        {
            return;
        }

        viewContext.Writer.Write(tagBuilder.RenderEndTag());
    }

    /// <summary>
    /// Renders the items within a content area. The rendering can optionally support additional row-based markup
    /// depending on the renderer options and the row support flag in the view data.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper used to render the content area items.</param>
    /// <param name="contentAreaItems">The collection of content area items to be rendered.</param>
    protected override void RenderContentAreaItems(IHtmlHelper htmlHelper, IEnumerable<ContentAreaItem> contentAreaItems)
    {
        var isRowSupported = htmlHelper.GetFlagValueFromViewData("rowsupport");
        var addRowMarkup = Options.RowSupportEnabled && isRowSupported.HasValue && isRowSupported.Value;

        // there is no need to proceed if row rendering support is disabled
        if (!addRowMarkup)
        {
            base.RenderContentAreaItems(htmlHelper, contentAreaItems);
            return;
        }

        var rowRender = new RowRenderer();
        rowRender.Render(contentAreaItems,
                         htmlHelper,
                         GetContentAreaItemTemplateTag,
                         GetColumnWidth,
                         base.RenderContentAreaItems);
    }

    private IContentData GetAreaItemContent(ContentAreaItem contentAreaItem)
    {
        if (contentAreaItem == null) return null;
        return _contentAreaLoader.LoadContent(contentAreaItem);
    }

    /// <summary>
    /// Renders a single content area item using the specified HTML elements and CSS class,
    /// ensuring proper rendering context management and handling of temporary HTML output.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper used for rendering the content area item.</param>
    /// <param name="contentAreaItem">The individual content area item to render.</param>
    /// <param name="templateTag">The template tag indicating the display option of the content area item.</param>
    /// <param name="htmlTag">The HTML tag to wrap the rendered content area item.</param>
    /// <param name="cssClass">The CSS class to apply to the wrapping HTML element.</param>
    protected override void RenderContentAreaItem(
        IHtmlHelper htmlHelper,
        ContentAreaItem contentAreaItem,
        string templateTag,
        string htmlTag,
        string cssClass)
    {
        var originalWriter = htmlHelper.ViewContext.Writer;
        var tempWriter = new HtmlStringWriter();
        htmlHelper.ViewContext.Writer = tempWriter;

        try
        {
            htmlHelper.ViewContext.ViewData[Constants.BlockIndexViewDataKey] =
                (int?)htmlHelper.ViewContext.ViewData[Constants.BlockIndexViewDataKey] + 1 ?? 0;

            var content = GetAreaItemContent(contentAreaItem);

            // persist selected DisplayOption for content template usage (if needed there of course)

            using (new ContentAreaItemContext(htmlHelper.ViewContext.ViewData, contentAreaItem))
            {
                // NOTE: if content area was rendered with tag (Html.PropertyFor(m => m.Area, new { tag = "..." }))
                // this tag is overridden if editor chooses display option for the block
                // therefore - we need to persist original CA tag and ask kindly EPiServer to render block template in original CA tag context
                var tag = string.IsNullOrEmpty(ContentAreaTag) ? templateTag : ContentAreaTag;

                base.RenderContentAreaItem(htmlHelper, contentAreaItem, tag, htmlTag, cssClass);

                var contentItemContent = tempWriter.ToString();
                var hasEditContainer = htmlHelper.GetFlagValueFromViewData(Constants.HasEditContainerKey);

                // we need to render block if we are in Edit mode
                if (IsInEditMode() && (hasEditContainer == null || hasEditContainer.Value))
                {
                    originalWriter.Write(contentItemContent);
                    return;
                }

                ProcessItemContent(contentItemContent, contentAreaItem, content, htmlHelper, originalWriter);
            }
        }
        finally
        {
            // restore original writer to proceed further with rendering pipeline
            htmlHelper.ViewContext.Writer = originalWriter;
        }
    }

    private void ProcessItemContent(
        string contentItemContent,
        ContentAreaItem contentAreaItem,
        IContentData content,
        IHtmlHelper htmlHelper,
        TextWriter originalWriter)
    {
        HtmlNode blockContentNode = null;

        var shouldStop = CallbackOnItemNode(contentItemContent, contentAreaItem, content, ref blockContentNode);
        if (shouldStop)
        {
            return;
        }

        shouldStop = RenderItemContainer(contentItemContent, htmlHelper, originalWriter, ref blockContentNode);
        if (shouldStop)
        {
            return;
        }

        shouldStop = ControlItemVisibility(contentItemContent, content, originalWriter, ref blockContentNode);
        if (shouldStop)
        {
            return;
        }

        // finally we just render whole body
        if (blockContentNode == null)
        {
            PrepareNodeElement(ref blockContentNode, contentItemContent);
        }

        if (blockContentNode != null)
        {
            originalWriter.Write(blockContentNode.OuterHtml);
        }
    }

    /// <summary>
    /// Retrieves the CSS class for a specified content area item, combining item-specific, tag-based, and base classes.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper used for rendering the content area item.</param>
    /// <param name="contentAreaItem">The content area item for which the CSS class will be retrieved.</param>
    /// <returns>The combined CSS class string for the specified content area item.</returns>
    protected override string GetContentAreaItemCssClass(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
    {
        return GetItemCssClass(htmlHelper, contentAreaItem);
    }

    internal string GetItemCssClass(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
    {
        var tag = GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem);
        var baseClasses = base.GetContentAreaItemCssClass(htmlHelper, contentAreaItem);

        return
            $"block {GetTypeSpecificCssClasses(contentAreaItem)}{(!string.IsNullOrEmpty(GetCssClassesForTag(contentAreaItem, tag)) ? " " + GetCssClassesForTag(contentAreaItem, tag) : "")}{(!string.IsNullOrEmpty(tag) ? " " + tag : "")}{(!string.IsNullOrEmpty(baseClasses) ? " " + baseClasses : "")}";
    }

    /// <summary>
    /// Retrieves the template tag for a specific content area item.
    /// The template tag is a string that defines how the item should be rendered.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper used to assist in rendering the content area item.</param>
    /// <param name="contentAreaItem">The content area item for which the template tag is determined.</param>
    /// <returns>A string representing the template tag associated with the content area item.</returns>
    protected override string GetContentAreaItemTemplateTag(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
    {
        return ContentAreaItemTemplateTagCore(htmlHelper, contentAreaItem);
    }

    internal string ContentAreaItemTemplateTagCore(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
    {
        var templateTag = base.GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem);
        if (!string.IsNullOrEmpty(templateTag)) { return templateTag; }

        // let's try to find default display options - when set to "Automatic" (meaning that tag is empty for the content)
        var currentContent = GetAreaItemContent(GetCurrentContent(contentAreaItem));
        var attribute = currentContent?.GetOriginalType().GetCustomAttribute<DefaultDisplayOptionAttribute>();

        if (attribute != null) { return attribute.DisplayOption; }

        // no default display option set in block definition using attributes
        // let's try to find - maybe developer set default one on CA definition
        return !string.IsNullOrEmpty(DefaultContentAreaDisplayOption)
            ? DefaultContentAreaDisplayOption
            : templateTag;
    }

    /// <summary>
    /// Returns the content area item currently being rendered, caching the instance for nested render calls.
    /// </summary>
    /// <param name="contentAreaItem">The item to resolve.</param>
    /// <returns>The active <see cref="ContentAreaItem"/>.</returns>
    protected virtual ContentAreaItem GetCurrentContent(ContentAreaItem contentAreaItem)
    {
        if (_currentContent == null || !_currentContent.ContentLink.CompareToIgnoreWorkID(contentAreaItem.ContentLink))
        {
            _currentContent = contentAreaItem;
        }

        return _currentContent;
    }

    internal int GetColumnWidth(string tag)
    {
        var fallback = _fallbacks.FirstOrDefault(f => f.Tag == tag);

        return fallback?.LargeScreenWidth ?? 12;
    }

    internal string GetCssClassesForTag(ContentAreaItem contentAreaItem, string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            tagName = ContentAreaTags.FullWidth;
        }

        // this is special case for skipping any CSS class calculations
        if (tagName.Equals(ContentAreaTags.None))
        {
            return string.Empty;
        }

        var extraTagInfo = string.Empty;

        // try to find default display option only if CA was rendered with tag
        // passed in tag is equal with tag used to render content area - block does not have any display option set explicitly
        if (!string.IsNullOrEmpty(ContentAreaTag) && tagName.Equals(ContentAreaTag))
        {
            // we also might have defined default display options for particular CA tag (Html.PropertyFor(m => m.ContentArea, new { tag = ... }))
            var currentContent = GetAreaItemContent(GetCurrentContent(contentAreaItem));
            var defaultAttribute = currentContent?.GetOriginalType()
                .GetCustomAttributes<DefaultDisplayOptionForTagAttribute>()
                .FirstOrDefault(a => a.Tag == ContentAreaTag);

            if (defaultAttribute != null)
            {
                tagName = defaultAttribute.DisplayOption;
                extraTagInfo = tagName;
            }
        }

        var fallback = _fallbacks.FirstOrDefault(f => f.Tag == tagName)
                       ?? _fallbacks.FirstOrDefault(f => f.Tag == ContentAreaTags.FullWidth);

        if (fallback == null)
        {
            return string.Empty;
        }

        return $"{GetCssClassesForItem(fallback)}{(string.IsNullOrEmpty(extraTagInfo) ? string.Empty : $" {extraTagInfo}")}";
    }

    // TODO: get rid of this static internal method and refactor to some sort of formatter / class builder / whatever
    // needed only for unittest to access this out of constructor - as there are lot of ceremony going on with injections (want to skip that).
    internal static string GetCssClassesForItem(DisplayModeFallback fallback)
    {
        var extraExtraLargeScreenClass = string.IsNullOrEmpty(fallback.ExtraExtraLargeScreenCssClassPattern)
            ? "col-xxl-" + fallback.ExtraExtraLargeScreenWidth
            : fallback.ExtraExtraLargeScreenCssClassPattern.TryFormat(fallback.ExtraExtraLargeScreenWidth);

        var extraLargeScreenClass = string.IsNullOrEmpty(fallback.ExtraLargeScreenCssClassPattern)
            ? "col-xl-" + fallback.ExtraLargeScreenWidth
            : fallback.ExtraLargeScreenCssClassPattern.TryFormat(fallback.ExtraLargeScreenWidth);

        var largeScreenClass = string.IsNullOrEmpty(fallback.LargeScreenCssClassPattern)
            ? "col-lg-" + fallback.LargeScreenWidth
            : fallback.LargeScreenCssClassPattern.TryFormat(fallback.LargeScreenWidth);

        var mediumScreenClass = string.IsNullOrEmpty(fallback.MediumScreenCssClassPattern)
            ? "col-md-" + fallback.MediumScreenWidth
            : fallback.MediumScreenCssClassPattern.TryFormat(fallback.MediumScreenWidth);

        var smallScreenClass = string.IsNullOrEmpty(fallback.SmallScreenCssClassPattern)
            ? "col-sm-" + fallback.SmallScreenWidth
            : fallback.SmallScreenCssClassPattern.TryFormat(fallback.SmallScreenWidth);

        var xsmallScreenClass = string.IsNullOrEmpty(fallback.ExtraSmallScreenCssClassPattern)
            ? "col-xs-" + fallback.ExtraSmallScreenWidth
            : fallback.ExtraSmallScreenCssClassPattern.TryFormat(fallback.ExtraSmallScreenWidth);

        return string.Join(
            " ",
            new[] { extraExtraLargeScreenClass, extraLargeScreenClass, largeScreenClass, mediumScreenClass, smallScreenClass, xsmallScreenClass }
                .Where(s => !string.IsNullOrEmpty(s)));
    }

    private string GetTypeSpecificCssClasses(ContentAreaItem contentAreaItem)
    {
        var content = GetAreaItemContent(contentAreaItem);
        var cssClass = content?.GetOriginalType().Name.ToLowerInvariant() ?? string.Empty;

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (content is ICustomCssInContentArea customClassContent && !string.IsNullOrWhiteSpace(customClassContent.ContentAreaCssClass))
        {
            cssClass += $" {customClassContent.ContentAreaCssClass}";
        }

        return cssClass;
    }

    private void PrepareNodeElement(ref HtmlNode node, string contentItemContent)
    {
        if (node != null)
        {
            return;
        }

        var doc = new HtmlDocument();
        doc.Load(new StringReader(contentItemContent));
        node = doc.DocumentNode.ChildNodes.FirstOrDefault();
    }

    private bool CallbackOnItemNode(
        string contentItemContent,
        ContentAreaItem contentAreaItem,
        IContentData content,
        ref HtmlNode blockContentNode)
    {
        // should we process start element node via callback?
        if (_elementStartTagRenderCallback == null)
        {
            return false;
        }

        PrepareNodeElement(ref blockContentNode, contentItemContent);
        if (blockContentNode == null)
        {
            return true;
        }

        // pass node to callback for some fancy modifications (if any)
        _elementStartTagRenderCallback.Invoke(blockContentNode, contentAreaItem, content);
        return false;
    }

    private bool RenderItemContainer(
        string contentItemContent,
        IHtmlHelper htmlHelper,
        TextWriter originalWriter,
        ref HtmlNode blockContentNode)
    {
        // do we need to control item container visibility?
        var renderItemContainer = htmlHelper.GetFlagValueFromViewData("hasitemcontainer");
        if (renderItemContainer.HasValue && !renderItemContainer.Value)
        {
            PrepareNodeElement(ref blockContentNode, contentItemContent);
            if (blockContentNode != null)
            {
                originalWriter.Write(blockContentNode.InnerHtml);
                return true;
            }
        }

        return false;
    }

    private bool ControlItemVisibility(
        string contentItemContent,
        IContentData content,
        TextWriter originalWriter,
        ref HtmlNode blockContentNode)
    {
        // can block be converted to IControlVisibility? so then we might need to control block rendering as such
        // ReSharper disable once SuspiciousTypeConversion.Global
        var visibilityControlledContent = content as IControlVisibility;
        if (visibilityControlledContent == null)
        {
            return false;
        }

        PrepareNodeElement(ref blockContentNode, contentItemContent);

        if (blockContentNode == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(blockContentNode.InnerHtml.Trim(null)) && visibilityControlledContent.HideIfEmpty)
        {
            return true;
        }

        originalWriter.Write(blockContentNode.OuterHtml);
        return true;
    }
}
