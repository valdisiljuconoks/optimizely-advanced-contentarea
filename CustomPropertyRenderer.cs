using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;

namespace EPiBootstrapArea
{
    public class CustomPropertyRenderer : PropertyRenderer
    {
        protected override MvcHtmlString GetHtmlForEditMode<TModel, TValue>(HtmlHelper<TModel> html,
                                                                            string viewModelPropertyName,
                                                                            object editorSettings,
                                                                            Func<string, MvcHtmlString> displayForAction,
                                                                            string templateName,
                                                                            string editElementName,
                                                                            string editElementCssClass,
                                                                            RouteValueDictionary additionalValues)
        {
            var hasEditContainer = additionalValues.GetFlagValue("HasEditContainer");
            if(hasEditContainer != null && !hasEditContainer.Value && html.ViewContext.RequestContext.IsInEditMode())
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

        private static MvcHtmlString CreateMvcHtmlString(Action<StringWriter> action)
        {
            using (var stringWriter = new StringWriter())
            {
                action(stringWriter);
                return new MvcHtmlString(stringWriter.ToString());
            }
        }
    }
}
