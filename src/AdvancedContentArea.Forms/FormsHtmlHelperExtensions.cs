using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using TechFellow.Optimizely.AdvancedContentArea.Initialization;
using TechFellow.Optimizely.AdvancedContentArea.Providers;

namespace TechFellow.Optimizely.AdvancedContentArea.Forms;

/// <summary>
/// Extension method enclosing class
/// </summary>
public static class FormsHtmlHelperExtensions
{
    /// <summary>
    /// Renders Optimizely.Forms elements respecting DisplayOptions
    /// </summary>
    /// <param name="html">Helper</param>
    /// <param name="currentStepIndex">Current index for the step</param>
    /// <param name="elements">Elements in current step</param>
    /// <param name="model">Model of the Forms Container page</param>
    /// <param name="additionalValues">Something that you would like to add</param>
    public static void RenderFormElements(
        this IHtmlHelper html,
        int currentStepIndex,
        IEnumerable<IFormElement> elements,
        FormContainerBlock model,
        object additionalValues = null)
    {
        // this means that somebody else took renderer seat and we need to find way around it
        // essentially the only thing that is needed is access to renderer instance - we can create one from scratch here also
        var renderer = ServiceLocator.Current.GetInstance<ContentAreaRenderer>() as AdvancedContentAreaRenderer
                       ?? new AdvancedContentAreaRenderer(new List<DisplayModeFallback>(DisplayOptions.Default),
                                                          new AdvancedContentAreaRendererOptions());

        var additionalParameters = new RouteValueDictionary(additionalValues);

        var isRowSupported = additionalParameters.GetValueFromDictionary("rowsupport");
        var addRowMarkup = renderer.Options.RowSupportEnabled && isRowSupported.HasValue && isRowSupported.Value;

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
        IHtmlHelper html,
        IEnumerable<ContentAreaItem> contentAreaItems,
        AdvancedContentAreaRenderer bootstrapAwareContentAreaRenderer,
        IEnumerable<IFormElement> formElements)
    {
        foreach (var item in contentAreaItems)
        {
            var formElement = formElements.FirstOrDefault(fe => fe.SourceContent.ContentLink == item.ContentLink);

            RenderAreaItem(html, item, bootstrapAwareContentAreaRenderer, formElement);
        }
    }

    private static void RenderAreaItem(
        IHtmlHelper html,
        ContentAreaItem contentAreaItem,
        AdvancedContentAreaRenderer renderer,
        IFormElement element)
    {
        if (contentAreaItem != null)
        {
            var cssClasses = renderer.GetItemCssClass(html, contentAreaItem);
            html.ViewContext.Writer.Write($"<div class=\"{cssClasses}\">");
        }

        var sourceContent = element?.SourceContent;
        if (sourceContent is { IsDeleted: false })
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
