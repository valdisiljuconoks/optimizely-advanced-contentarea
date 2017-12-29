using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Web.Mvc.Html;

namespace EPiBootstrapArea.Forms
{
    public static class FormsHtmlHelperExtensions
    {
        public static void RenderFormElements(this HtmlHelper html, int currentStepIndex, IEnumerable<IFormElement> elements, FormContainerBlock model)
        {
            // TODO: this is pretty scary - another approach would be to ask from container and then try to cast to this type..
            var renderer = new BootstrapAwareContentAreaRenderer();

            foreach (var element in elements)
            {
                var areaItem = model.ElementsArea.Items.FirstOrDefault(i => i.ContentLink == element.SourceContent.ContentLink);

                if(areaItem != null)
                {
                    var cssClasses = renderer.GetItemCssClass(html, areaItem);
                    html.ViewContext.Writer.Write($"<div class=\"{cssClasses}\">");
                }

                var sourceContent = element.SourceContent;
                if(sourceContent != null && !sourceContent.IsDeleted)
                {
                    if(sourceContent is ISubmissionAwareElement)
                    {
                        var contentData = (sourceContent as IReadOnly).CreateWritableClone() as IContent;
                        (contentData as ISubmissionAwareElement).FormSubmissionId = (string) html.ViewBag.FormSubmissionId;
                        html.RenderContentData(contentData, false);
                    }
                    else
                    {
                        html.RenderContentData(sourceContent, false);
                    }
                }

                if(areaItem != null)
                    html.ViewContext.Writer.Write("</div>");
            }
        }
    }
}
