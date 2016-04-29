using System;

namespace EPiBootstrapArea
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultDisplayOptionAttribute : Attribute
    {
        public DefaultDisplayOptionAttribute(string displayOption)
        {
            DisplayOption = displayOption;
        }

        public string DisplayOption { get; private set; }
    }
}
