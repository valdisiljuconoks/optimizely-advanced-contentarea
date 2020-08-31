using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using EPiBootstrapArea.Initialization;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;

namespace EPiBootstrapArea.Forms
{
    public static class FormsHtmlHelperExtensions
    {
        public static void RenderFormElements(
            this HtmlHelper html,
            int currentStepIndex,
            IEnumerable<IFormElement> elements,
            FormContainerBlock model,
            object additionalValues = null)
        {
            // this means that somebody else took renderer seat and we need to find way around it
            // essentially the only thing that is needed is access to renderer instance - we can create one from scratch here also
            var renderer = ServiceLocator.Current.GetInstance<ContentAreaRenderer>() as BootstrapAwareContentAreaRenderer
                           ?? new BootstrapAwareContentAreaRenderer(SetupBootstrapRenderer.AllDisplayOptions);

            var additionalParameters = new RouteValueDictionary(additionalValues);

            var isRowSupported = additionalParameters.GetValueFromDictionary("rowsupport");
            var addRowMarkup = ConfigurationContext.Current.RowSupportEnabled && isRowSupported.HasValue && isRowSupported.Value;

            if (!addRowMarkup)
            {
                foreach (var element in elements)
                {
                    var areaItem = model
                        .ElementsArea
                        .Items
                        .FirstOrDefault(i => i.ContentLink == element.SourceContent.ContentLink);

                    RenderAreaItem(html, areaItem, renderer, element);
                }
            }
            else
            {
                var rowRenderer = new RowRenderer();
                rowRenderer.Render(
                    model.ElementsArea.Items,
                    html,
                    renderer.ContentAreaItemTemplateTagCore,
                    renderer.GetColumnWidth,
                    (_, items) => RenderItems(html, items, renderer, elements));
            }
        }

        private static void RenderItems(
            HtmlHelper html,
            IEnumerable<ContentAreaItem> contentAreaItems,
            BootstrapAwareContentAreaRenderer bootstrapAwareContentAreaRenderer,
            IEnumerable<IFormElement> formElements)
        {
            foreach (var item in contentAreaItems)
            {
                var formElement = formElements.FirstOrDefault(fe => fe.SourceContent.ContentLink == item.ContentLink);

                RenderAreaItem(html, item, bootstrapAwareContentAreaRenderer, formElement);
            }
        }

        private static void RenderAreaItem(
            HtmlHelper html,
            ContentAreaItem contentAreaItem,
            BootstrapAwareContentAreaRenderer renderer,
            IFormElement element)
        {
            if (contentAreaItem != null)
            {
                var cssClasses = renderer.GetItemCssClass(html, contentAreaItem);
                html.ViewContext.Writer.Write($"<div class=\"{cssClasses}\">");
            }

            var sourceContent = element.SourceContent;
            if (sourceContent != null && !sourceContent.IsDeleted)
            {
                if (sourceContent is ISubmissionAwareElement)
                {
                    var contentData = (sourceContent as IReadOnly).CreateWritableClone() as IContent;
                    (contentData as ISubmissionAwareElement).FormSubmissionId = (string)html.ViewBag.FormSubmissionId;
                    html.RenderContentData(contentData, false);
                }
                else
                {
                    html.RenderContentData(sourceContent, false);
                }
            }

            if (contentAreaItem != null)
            {
                html.ViewContext.Writer.Write("</div>");
            }
        }
    }
}
