using System;
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
            var hiddenInEditMode = additionalValues.GetFlagValue("HiddenInEditMode");
            if(hiddenInEditMode != null && hiddenInEditMode.Value && html.ViewContext.RequestContext.IsInEditMode())
            {
                return MvcHtmlString.Empty;
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
    }
}
