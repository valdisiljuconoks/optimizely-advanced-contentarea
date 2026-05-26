// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Marks content that should be hidden in the content area when it renders no output.
/// </summary>
public interface IControlVisibility
{
    /// <summary>
    /// When <see langword="true"/>, the renderer omits the block wrapper when the rendered output is empty.
    /// </summary>
    bool HideIfEmpty { get; }
}
