using Xunit;
using CateringNotification.Content.Campus;
using System.Threading.Tasks;

namespace CateringNotification.Tests
{
    public class CampusTest
    {
        [Theory]
        [InlineData("https://www.pxl.be/Pub/Studenten/Voorzieningen-Student/Catering/")]
        [InlineData(null)]
        [InlineData("")]
        public async Task Campus_GetMenuAsync_ShouldReturnNullWhenInputIsInvalid(string url)
        {
            //Act
            var result = await Campus.GetMenuAsync(url);

            //Assert
            Assert.Null(result);
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
            Assert.True(result.Length > 155);
        }
    }
}
