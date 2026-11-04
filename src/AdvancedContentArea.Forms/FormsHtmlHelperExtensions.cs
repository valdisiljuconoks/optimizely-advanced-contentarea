// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using TechFellow.Optimizely.AdvancedContentArea.Initialization;
using TechFellow.Optimizely.AdvancedContentArea.Providers;

namespace TechFellow.Optimizely.AdvancedContentArea.Forms;

/// <summary>
/// Extension method enclosing class
/// </summary>
public static class FormsHtmlHelperExtensions
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(FormsHtmlHelperExtensions));

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

        // pair this step's elements with their content area items (display options live on the item).
        // same skip rule as the stock `RenderElementsInStep`, and the item is matched ignoring the version - like
        // Forms does - so draft element versions in preview still find theirs
        var stepItems = elements
            .Where(e => e.SourceContent is { IsDeleted: false })
            .Select(e => (Element: e, Item: model.ElementsArea.Items.FirstOrDefault(i => e.SourceContent.ContentLink.CompareToIgnoreWorkID(i.ContentLink))))
            .ToList();

        if (!addRowMarkup)
        {
            foreach (var (element, item) in stepItems)
            {
                RenderAreaItem(html, item, renderer, element);
            }

            return;
        }

        // only this step's items - ElementsArea holds the elements of every step plus the step blocks themselves
        new RowRenderer().Render(
            stepItems.Where(p => p.Item != null).Select(p => p.Item),
            html,
            renderer.ContentAreaItemTemplateTagCore,
            renderer.GetColumnWidth,
            (_, items) =>
            {
                foreach (var item in items)
                {
                    RenderAreaItem(html, item, renderer, stepItems.First(p => p.Item == item).Element);
                }
            });
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

        var sourceContent = element.SourceContent;

        try
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
        catch (Exception ex)
        {
            // same as the stock `RenderElementsInStep` - one broken element must not take the whole form (and page) down
            Logger.Error($"Exception thrown when rendering {sourceContent.Name}", ex);
        }

        if (contentAreaItem != null)
        {
            html.ViewContext.Writer.Write("</div>");
        }
    }
}
