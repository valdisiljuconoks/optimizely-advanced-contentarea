using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;

namespace EPiBootstrapArea
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class BootstrapRowValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var contentArea = value as ContentArea;
            var noItems = contentArea?.Items == null;

            if(noItems)
                return false;

            var count = 0;
            foreach (var item in contentArea.Items)
            {
                var displayOption = item.LoadDisplayOption();

                if(displayOption == null)
                    continue;

                var optionAsEnum = GetDisplayOptionTag(displayOption.Tag);
                count = count + optionAsEnum;

                if(count > 12)
                    return false;
            }

            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = base.IsValid(value, validationContext);

            if(!string.IsNullOrWhiteSpace(result?.ErrorMessage))
                result.ErrorMessage = "Items exceed all 12 Bootstrap columns";

            return result;
        }

        public static int GetDisplayOptionTag(string tag)
        {
            return BootstrapAwareContentAreaRenderer.GetColumnWidth(tag);
        }
    }
}
