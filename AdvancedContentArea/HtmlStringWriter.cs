// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// A <see cref="StringWriter"/> that encodes <see cref="IHtmlContent"/> values instead of calling <see cref="object.ToString"/>.
/// </summary>
public class HtmlStringWriter : StringWriter
{
    /// <inheritdoc />
    public override void Write(object value)
    {
        if (value is IHtmlContent content)
        {
            content.WriteTo(this, HtmlEncoder.Default);
        }
        else
        {
            base.Write(value);
        }
    }
}
