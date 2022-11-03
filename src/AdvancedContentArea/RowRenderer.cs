using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechFellow.Optimizely.AdvancedContentArea
{
    public class RowRenderer
    {
        public void Render(
            IEnumerable<ContentAreaItem> contentAreaItems,
            IHtmlHelper htmlHelper,
            Func<IHtmlHelper, ContentAreaItem, string> getTemplateTag,
            Func<string, int> getColumnWidth,
            Action<IHtmlHelper, IEnumerable<ContentAreaItem>> renderItems)
        {
            var items = contentAreaItems.ToList();
            var currentRow = 0;
            var rowWidthState = 0;

            var itemInfos = items.Select(item =>
            {
                var tag = getTemplateTag(htmlHelper, item);
                var columnWidth = getColumnWidth(tag);

                if (rowWidthState + columnWidth > 12)
                {
                    currentRow++;
                    rowWidthState = columnWidth;
                }
                else
                {
                    rowWidthState += columnWidth;
                }

                return new
                {
                    ContentAreaItem = item,
                    Tag = tag,
                    ColumnWidth = columnWidth,
                    RowWidthState = rowWidthState,
                    RowNumber = currentRow
                };
            }).ToList();

            var rows = itemInfos.GroupBy(a => a.RowNumber, a => a.ContentAreaItem);
            foreach (var row in rows)
            {
                var originalWriter = htmlHelper.ViewContext.Writer;
                var tempWriter = new StringWriter();
                htmlHelper.ViewContext.Writer = tempWriter;

                try
                {
                    renderItems(htmlHelper, row);
                    var itemContent = htmlHelper.ViewContext.Writer.ToString();

                    if (!string.IsNullOrEmpty(itemContent))
                    {
                        var rowClass = htmlHelper.GetValueFromViewData("rowcssclass");

                        originalWriter.Write($"<div class=\"row row{row.Key}{(!string.IsNullOrEmpty(rowClass)? " " + rowClass : string.Empty)}\">");
                        originalWriter.Write(itemContent);
                        originalWriter.Write("</div>");
                    }
                }
                finally
                {
                    htmlHelper.ViewContext.Writer = originalWriter;
                }
            }
        }
    }
}
