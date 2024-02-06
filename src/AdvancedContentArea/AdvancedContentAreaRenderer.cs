// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Mvc.Html;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechFellow.Optimizely.AdvancedContentArea.Extensions;
using TechFellow.Optimizely.AdvancedContentArea.Initialization;
using TechFellow.Optimizely.AdvancedContentArea.Providers;

namespace TechFellow.Optimizely.AdvancedContentArea;

public class AdvancedContentAreaRenderer : ContentAreaRenderer
{
    private IContent _currentContent;
    private Action<HtmlNode, ContentAreaItem, IContent> _elementStartTagRenderCallback;
    private IEnumerable<DisplayModeFallback> _fallbacks;
    internal readonly AdvancedContentAreaRendererOptions Options;

    public AdvancedContentAreaRenderer(IReadOnlyCollection<DisplayModeFallback> fallbacks, AdvancedContentAreaRendererOptions options)
    {
        _fallbacks = fallbacks ?? throw new ArgumentNullException(nameof(fallbacks));
        Options = options;
    }

    public string ContentAreaTag { get; private set; }

    public string DefaultContentAreaDisplayOption { get; private set; }

    internal void SetElementStartTagRenderCallback(Action<HtmlNode, ContentAreaItem, IContent> callback)
    {
        _elementStartTagRenderCallback = callback;
    }

    internal void SetDisplayOptions(List<DisplayModeFallback> displayOptions)
    {
        _fallbacks = displayOptions;
    }

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

        RenderContentAreaItems(htmlHelper, contentArea.FilteredItems);

        if (tagBuilder == null)
        {
            return;
        }

        viewContext.Writer.Write(tagBuilder.RenderEndTag());
    }

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

            var content = contentAreaItem.GetContent();

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
        IContent content,
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

    protected override string GetContentAreaItemTemplateTag(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
    {
        return ContentAreaItemTemplateTagCore(htmlHelper, contentAreaItem);
    }

    internal string ContentAreaItemTemplateTagCore(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
    {
        var templateTag = base.GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem);
        if (!string.IsNullOrEmpty(templateTag)) { return templateTag; }

        // let's try to find default display options - when set to "Automatic" (meaning that tag is empty for the content)
        var currentContent = GetCurrentContent(contentAreaItem);
        var attribute = currentContent?.GetOriginalType().GetCustomAttribute<DefaultDisplayOptionAttribute>();

        if (attribute != null) { return attribute.DisplayOption; }

        // no default display option set in block definition using attributes
        // let's try to find - maybe developer set default one on CA definition
        return !string.IsNullOrEmpty(DefaultContentAreaDisplayOption)
            ? DefaultContentAreaDisplayOption
            : templateTag;
    }

    protected virtual IContent GetCurrentContent(ContentAreaItem contentAreaItem)
    {
        if (_currentContent == null || !_currentContent.ContentLink.CompareToIgnoreWorkID(contentAreaItem.ContentLink))
        {
            _currentContent = contentAreaItem.GetContent();
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
            var currentContent = GetCurrentContent(contentAreaItem);
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

    private static string GetTypeSpecificCssClasses(ContentAreaItem contentAreaItem)
    {
        var content = contentAreaItem.GetContent();
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
        IContent content,
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
        IContent content,
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
