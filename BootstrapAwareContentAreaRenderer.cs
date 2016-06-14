using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using EPiBootstrapArea.Providers;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using HtmlAgilityPack;

namespace EPiBootstrapArea
{
    public class BootstrapAwareContentAreaRenderer : ContentAreaRenderer
    {
        private static bool _fallbackCached;
        private static IEnumerable<DisplayModeFallback> _fallbacks;
        private IContent _currentContent;
        private Action<HtmlNode, ContentAreaItem, IContent> _elementStartTagRenderCallback;

        public BootstrapAwareContentAreaRenderer()
        {
            ReadRegisteredDisplayModes();
        }

        public bool RowSupportEnabled { get; set; }

        public bool AutoAddRow { get; set; }

        public string ContentAreaTag { get; private set; }

        public string DefaultContentAreaDisplayOption { get; private set; }

        protected void SetElementStartTagRenderCallback(Action<HtmlNode, ContentAreaItem, IContent> callback)
        {
            _elementStartTagRenderCallback = callback;
        }

        public override void Render(HtmlHelper htmlHelper, ContentArea contentArea)
        {
            if(contentArea == null || contentArea.IsEmpty)
            {
                return;
            }

            // capture given CA tag (should be contentArea.Tag, but EPiServer is not filling that property)
            ContentAreaTag = htmlHelper.ViewData["tag"] as string;
            if(htmlHelper.ViewData.ModelMetadata.AdditionalValues.ContainsKey($"{nameof(DefaultDisplayOptionMetadataProvider)}__DefaultDisplayOption"))
            {
                DefaultContentAreaDisplayOption = htmlHelper.ViewData.ModelMetadata.AdditionalValues[$"{nameof(DefaultDisplayOptionMetadataProvider)}__DefaultDisplayOption"] as string;
            }

            var viewContext = htmlHelper.ViewContext;
            TagBuilder tagBuilder = null;

            if(!IsInEditMode(htmlHelper) && ShouldRenderWrappingElement(htmlHelper))
            {
                tagBuilder = new TagBuilder(GetContentAreaHtmlTag(htmlHelper, contentArea));
                AddNonEmptyCssClass(tagBuilder, viewContext.ViewData["cssclass"] as string);

                if(AutoAddRow)
                {
                    AddNonEmptyCssClass(tagBuilder, "row");
                }

                viewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            }

            RenderContentAreaItems(htmlHelper, contentArea.FilteredItems);

            if(tagBuilder == null)
            {
                return;
            }

            viewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.EndTag));
        }

        protected override void RenderContentAreaItems(HtmlHelper htmlHelper, IEnumerable<ContentAreaItem> contentAreaItems)
        {
            var isRowSupported = htmlHelper.GetFlagValueFromViewData("rowsupport");
            var addRowMarkup = (!isRowSupported.HasValue && RowSupportEnabled) || (isRowSupported ?? false);

            // there is no need to proceed if row rendering support is disabled
            if(!addRowMarkup)
            {
                base.RenderContentAreaItems(htmlHelper, contentAreaItems);
                return;
            }

            var items = contentAreaItems.ToList();
            var rowWidthState = 0;
            var itemInfos = items.Select(item =>
                                         {
                                             var tag = GetContentAreaItemTemplateTag(htmlHelper, item);
                                             var columnWidth = GetColumnWidth(tag);
                                             rowWidthState += columnWidth;
                                             return new
                                                    {
                                                        ContentAreaItem = item,
                                                        Tag = tag,
                                                        ColumnWidth = columnWidth,
                                                        RowWidthState = rowWidthState,
                                                        RowNumber = rowWidthState % 12 == 0 ? rowWidthState / 12 - 1 : rowWidthState / 12
                                                    };
                                         }).ToList();

            // if tags exists wrap items with row or not then use the default rendering
            var tagExists = itemInfos.Any(ii => !string.IsNullOrEmpty(ii.Tag));
            if(!tagExists)
            {
                base.RenderContentAreaItems(htmlHelper, items);
                return;
            }

            var rows = itemInfos.GroupBy(a => a.RowNumber, a => a.ContentAreaItem);
            foreach (var row in rows)
            {
                htmlHelper.ViewContext.Writer.Write("<div class=\"row row" + row.Key + "\">");
                base.RenderContentAreaItems(htmlHelper, row);
                htmlHelper.ViewContext.Writer.Write("</div>");
            }
        }

        protected override void RenderContentAreaItem(
            HtmlHelper htmlHelper,
            ContentAreaItem contentAreaItem,
            string templateTag,
            string htmlTag,
            string cssClass)
        {
            var originalWriter = htmlHelper.ViewContext.Writer;
            var tempWriter = new StringWriter();
            htmlHelper.ViewContext.Writer = tempWriter;

            try
            {
                var content = contentAreaItem.GetContent(ContentRepository);

                // NOTE: if content area was rendered with tag (Html.PropertyFor(m => m.Area, new { tag = "..." }))
                // this tag is overridden if editor chooses display option for the block
                // therefore - we need to persist original CA tag and ask kindly EPiServer to render block template in original CA tag context
                var tag = string.IsNullOrEmpty(ContentAreaTag) ? templateTag : ContentAreaTag;

                base.RenderContentAreaItem(htmlHelper, contentAreaItem, tag, htmlTag, cssClass);
                var contentItemContent = tempWriter.ToString();
                var hasEditContainer = htmlHelper.GetFlagValueFromViewData("HasEditContainer");

                // we need to render block if we are in Edit mode
                if (IsInEditMode(htmlHelper) && (hasEditContainer == null || hasEditContainer.Value))
                {
                    originalWriter.Write(contentItemContent);
                    return;
                }

                ProcessItemContent(contentItemContent, contentAreaItem, content, htmlHelper, originalWriter);
            }
            finally
            {
                // restore original writer to proceed further with rendering pipeline
                htmlHelper.ViewContext.Writer = originalWriter;
            }
        }

        private void ProcessItemContent(string contentItemContent, ContentAreaItem contentAreaItem, IContent content, HtmlHelper htmlHelper, TextWriter originalWriter)
        {
            HtmlNode blockContentNode = null;

            var shouldStop = CallbackOnItemNode(contentItemContent, contentAreaItem, content, ref blockContentNode);
            if(shouldStop)
                return;

            shouldStop = RenderItemContainer(contentItemContent, htmlHelper, originalWriter, ref blockContentNode);
            if(shouldStop)
                return;

            shouldStop = ControlItemVisibility(contentItemContent, content, originalWriter, ref blockContentNode);
            if(shouldStop)
                return;

            // finally we just render whole body
            originalWriter.Write(contentItemContent);
        }

        protected override string GetContentAreaItemCssClass(HtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
        {
            var tag = GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem);
            var baseClasses = base.GetContentAreaItemCssClass(htmlHelper, contentAreaItem);

            return string.Format("block {0} {1} {2} {3}",
                                 GetTypeSpecificCssClasses(contentAreaItem, ContentRepository),
                                 GetCssClassesForTag(contentAreaItem, tag),
                                 tag,
                                 baseClasses);
        }

        protected override string GetContentAreaItemTemplateTag(HtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
        {
            var templateTag = base.GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem);
            if(!string.IsNullOrEmpty(templateTag))
            {
                return templateTag;
            }

            // let's try to find default display options - when set to "Automatic" (meaning that tag is empty for the content)
            var currentContent = GetCurrentContent(contentAreaItem);
            var attribute = currentContent.GetOriginalType().GetCustomAttribute<DefaultDisplayOptionAttribute>();

            if(attribute != null)
            {
                return attribute.DisplayOption;
            }

            // no default display option set in block definition using attributes
            // let's try to find - maybe developer set default one on CA definition
            return !string.IsNullOrEmpty(DefaultContentAreaDisplayOption)
                       ? DefaultContentAreaDisplayOption
                       : templateTag;
        }

        protected virtual IContent GetCurrentContent(ContentAreaItem contentAreaItem)
        {
            if(_currentContent == null || !_currentContent.ContentLink.CompareToIgnoreWorkID(contentAreaItem.ContentLink))
            {
                _currentContent = contentAreaItem.GetContent(ContentRepository);
            }

            return _currentContent;
        }

        private static int GetColumnWidth(string tag)
        {
            var fallback = _fallbacks.FirstOrDefault(f => f.Tag == tag);
            return fallback?.LargeScreenWidth ?? 12;
        }

        private string GetCssClassesForTag(ContentAreaItem contentAreaItem, string tagName)
        {
            if(string.IsNullOrWhiteSpace(tagName))
            {
                tagName = ContentAreaTags.FullWidth;
            }

            var extraTagInfo = string.Empty;

            // try to find default display option only if CA was rendered with tag
            // passed in tag is equal with tag used to render content area - block does not have any display option set explicitly
            if(!string.IsNullOrEmpty(ContentAreaTag) && tagName.Equals(ContentAreaTag))
            {
                // we also might have defined default display options for particular CA tag (Html.PropertyFor(m => m.ContentArea, new { tag = ... }))
                var defaultAttribute = GetCurrentContent(contentAreaItem).GetOriginalType()
                                                                         .GetCustomAttributes<DefaultDisplayOptionForTagAttribute>()
                                                                         .FirstOrDefault(a => a.Tag == ContentAreaTag);

                if(defaultAttribute != null)
                {
                    tagName = defaultAttribute.DisplayOption;
                    extraTagInfo = tagName;
                }
            }

            var fallback = _fallbacks.FirstOrDefault(f => f.Tag == tagName)
                           ?? _fallbacks.FirstOrDefault(f => f.Tag == ContentAreaTags.FullWidth);

            if(fallback == null)
            {
                return string.Empty;
            }

            return string.Format("col-lg-{0} col-md-{1} col-sm-{2} col-xs-{3}{4}",
                                 fallback.LargeScreenWidth,
                                 fallback.MediumScreenWidth,
                                 fallback.SmallScreenWidth,
                                 fallback.ExtraSmallScreenWidth,
                                 string.IsNullOrEmpty(extraTagInfo) ? string.Empty : $" {extraTagInfo}");
        }

        private static string GetTypeSpecificCssClasses(ContentAreaItem contentAreaItem, IContentLoader contentLoader)
        {
            var content = contentAreaItem.GetContent(contentLoader);
            var cssClass = content == null ? string.Empty : content.GetOriginalType().Name.ToLowerInvariant();

            // ReSharper disable once SuspiciousTypeConversion.Global
            var customClassContent = content as ICustomCssInContentArea;
            if(customClassContent != null && !string.IsNullOrWhiteSpace(customClassContent.ContentAreaCssClass))
            {
                cssClass += $" {customClassContent.ContentAreaCssClass}";
            }

            return cssClass;
        }

        private static void ReadRegisteredDisplayModes()
        {
            if(_fallbackCached)
            {
                return;
            }

            var displayModeFallbackProvider = ServiceLocator.Current.GetInstance<IDisplayModeFallbackProvider>();
            _fallbacks = displayModeFallbackProvider.GetAll();
            _fallbackCached = true;
        }

        private void PrepareNodeElement(ref HtmlNode node, string contentItemContent)
        {
            if(node != null)
            {
                return;
            }

            var doc = new HtmlDocument();
            doc.Load(new StringReader(contentItemContent));
            node = doc.DocumentNode.ChildNodes.FirstOrDefault();
        }

        private bool CallbackOnItemNode(string contentItemContent, ContentAreaItem contentAreaItem, IContent content, ref HtmlNode blockContentNode)
        {
            // should we process start element node via callback?
            if(_elementStartTagRenderCallback == null)
            {
                return false;
            }

            PrepareNodeElement(ref blockContentNode, contentItemContent);
            if(blockContentNode == null)
            {
                return true;
            }

            // pass node to callback for some fancy modifications (if any)
            _elementStartTagRenderCallback.Invoke(blockContentNode, contentAreaItem, content);
            return false;
        }

        private bool RenderItemContainer(string contentItemContent, HtmlHelper htmlHelper, TextWriter originalWriter, ref HtmlNode blockContentNode)
        {
            // do we need to control item container visibility?
            var renderItemContainer = htmlHelper.GetFlagValueFromViewData("hasitemcontainer");
            if(renderItemContainer.HasValue && !renderItemContainer.Value)
            {
                PrepareNodeElement(ref blockContentNode, contentItemContent);
                if(blockContentNode != null)
                {
                    originalWriter.Write(blockContentNode.InnerHtml);
                    return true;
                }
            }

            return false;
        }

        private bool ControlItemVisibility(string contentItemContent, IContent content, TextWriter originalWriter, ref HtmlNode blockContentNode)
        {
            // can block be converted to IControlVisibility? so then we might need to control block rendering as such
            // ReSharper disable once SuspiciousTypeConversion.Global
            var visibilityControlledContent = content as IControlVisibility;
            if(visibilityControlledContent == null)
                return false;

            PrepareNodeElement(ref blockContentNode, contentItemContent);
            if(blockContentNode != null && !visibilityControlledContent.HideIfEmpty)
            {
                originalWriter.Write(blockContentNode.OuterHtml);
                return true;
            }

            return false;
        }
    }
}
