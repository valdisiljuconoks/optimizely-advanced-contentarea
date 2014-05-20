using System;
using Xunit;
using Xunit.Extensions;

namespace EPiBootstrapArea.Tests
{
    public class ValidationTests
    {
        

        [Theory]
        [InlineData("name", 12, 12, 12, 12, "tag", true)]
        [InlineData("name", 1, 1, 1, 1, "tag", true)]
        [InlineData(null, 1, 1, 1, 1, "tag", false)]
        [InlineData("name", 20, 20, 20, 20, "tag", false)]
        [InlineData("name", 0, 0, 0, 0, "tag", false)]
        [InlineData("", 0, 0, 0, 0, "tag", false)]
        public void TestSomeValidation(string name, int large, int medium, int small, int xsmall, string tag, bool validationResult)
        {
            var mode = new DisplayModeFallback
                       {
                           Name = name,
                           LargeScreenWidth = large,
                           MediumScreenWidth = medium,
                           SmallScreenWidth = small,
                           ExtraSmallScreenWidth = xsmall,
                           Tag = tag,
                       };

            bool result;

            try
            {
                Registrar.ValidateModes(mode);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            Assert.Equal(validationResult, result);
        }
    }
}
