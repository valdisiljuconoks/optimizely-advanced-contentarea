// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;

namespace TechFellow.Optimizely.AdvancedContentArea;

/// <summary>
/// Supplies Bootstrap display mode fallbacks for the advanced content area renderer.
/// </summary>
public interface IDisplayModeFallbackProvider
{
    /// <summary>
    /// Performs one-time setup before display modes are read.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Returns all configured display mode fallbacks.
    /// </summary>
    /// <returns>A list of <see cref="DisplayModeFallback"/> definitions.</returns>
    List<DisplayModeFallback> GetAll();
}
