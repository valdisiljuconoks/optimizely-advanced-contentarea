using System;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Web;

namespace EPiBootstrapArea
{
    internal class ContentAreaItemContext : IDisposable
    {
        private readonly ViewDataDictionary _viewData;

        public ContentAreaItemContext(ViewDataDictionary viewData, ContentAreaItem contentAreaItem)
        {
            _viewData = viewData;
            var displayOption = contentAreaItem.LoadDisplayOption() ?? new DisplayOption
                                                                       {
                                                                           Id = Guid.NewGuid().ToString(),
                                                                           Name = "Unknown"
                                                                       };

            if (!_viewData.ContainsKey(Constants.CurrentDisplayOptionKey))
            {
                _viewData.Add(Constants.CurrentDisplayOptionKey, displayOption);
            }
            else
            {
                _viewData[Constants.CurrentDisplayOptionKey] = displayOption;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!disposing)
                return;

            _viewData.Remove(Constants.CurrentDisplayOptionKey);
        }
    }
}
