using Xunit;
using CateringNotification.Content.Campus;

namespace CateringNotification.Tests
{
    public class CampusTest
    {
        [Theory]
        [InlineData("https://www.pxl.be/Pub/Studenten/Voorzieningen-Student/Catering/")]
        [InlineData(null)]
        [InlineData("")]
        public void Campus_GetMenuAsync_ShouldContain18CharactersWhenInputIsInvalid(string url)
        {
            //Act
            var result = Campus.GetMenuAsync(url).Result;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(18, result.Length);
        }

        [Fact]
        public void Campus_GetMenuAsync_ShouldContainMoreThan18Characters()
        {
            //Act
            var result = Campus
                .GetMenuAsync("https://www.pxl.be/Pub/Studenten/Voorzieningen-Student/Catering/Weekmenu-Campus-Elfde-Linie.html")
                .Result;

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 18);
        }
    }
}
