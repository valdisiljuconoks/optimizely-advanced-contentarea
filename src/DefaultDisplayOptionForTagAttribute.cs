using System;

namespace EPiBootstrapArea
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DefaultDisplayOptionForTagAttribute : Attribute
    {
        public DefaultDisplayOptionForTagAttribute(string tag, string displayOption)
        {
            if(string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if(string.IsNullOrWhiteSpace(displayOption))
            {
                throw new ArgumentNullException(nameof(displayOption));
            }

            Tag = tag;
            DisplayOption = displayOption;
        }

        public string Tag { get; private set; }

        public string DisplayOption { get; private set; }
    }
}
