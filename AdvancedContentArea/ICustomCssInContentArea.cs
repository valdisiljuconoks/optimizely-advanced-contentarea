// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Allows a content type to supply an extra CSS class for its content area item wrapper.
/// </summary>
public interface ICustomCssInContentArea
{
    /// <summary>
    /// Additional CSS class names applied to the content area item element.
    /// </summary>
    string ContentAreaCssClass { get; }
}
