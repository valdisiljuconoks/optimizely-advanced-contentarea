using System;
using System.IO;
using EPiServer.Web.Mvc.Html;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace EPiBootstrapArea
{
    public class CustomPropertyRenderer : PropertyRenderer
    {
        protected override IHtmlContent GetHtmlForEditMode<TModel, TValue>(
            IHtmlHelper<TModel> html,
            string viewModelPropertyName,
            object editorSettings,
            Func<string, IHtmlContent> displayForAction,
            string templateName,
            string editElementName,
            string editElementCssClass,
            RouteValueDictionary additionalValues)
        {
            var hasEditContainer = additionalValues.GetFlagValue(Constants.HasEditContainerKey);
            if (hasEditContainer != null && !hasEditContainer.Value && html.ViewContext.IsInEditMode())
            {
                return CreateMvcHtmlString(writer => writer.Write(displayForAction(templateName)));
            }

            return base.GetHtmlForEditMode<TModel, TValue>(html,
                                                           viewModelPropertyName,
                                                           editorSettings,
                                                           displayForAction,
                                                           templateName,
                                                           editElementName,
                                                           editElementCssClass,
                                                           additionalValues);
        }

        private static HtmlString CreateMvcHtmlString(Action<StringWriter> action)
        {
            using var stringWriter = new StringWriter();
            action(stringWriter);

            return new HtmlString(stringWriter.ToString());
        }
    }
}
