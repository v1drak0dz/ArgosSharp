using ArgosSharp.Application.Interfaces.Parser;
using ArgosSharp.Infrastructure.Http.Parser;
using FluentAssertions;
using Moq;

namespace ArgosSharp.Infrastructure.UnitTests.Http.Parser
{
    public class AngleSharpHtmlParserTests
    {
        private IHtmlParser _htmlParser;

        [SetUp]
        public void SetUp()
        {
            _htmlParser = new AngleSharpHtmlParser();
        }

        [Test]
        public void QueryText_WhenValidSelector_ReturnsText()
        {
            // Arrange
            var html = "<html><body><h1>Hello World</h1></body></html>";
            var selector = "h1";
            var expected = "Hello World";

            // Act
            var result = _htmlParser.QueryText(html, selector);

            // Assert
            result.Should().Be(expected);
        }

        [Test]
        public void QueryTexts_WhenMultipleElements_ReturnsAllTexts()
        {
            // Arrange
            var html = @"
                <html>
                    <body>
                        <p>Item 1</p>
                        <p>Item 2</p>
                        <p>Item 3</p>
                    </body>
                </html>";

            var selector = "p";

            // Act
            var result = _htmlParser.QueryTexts(html, selector);

            // Assert
            result.Should().BeEqualTo(["Item 1", "Item 2", "Item 3"]);
        }

        [Test]
        public void QueryText_WhenSelectorNotFound_ReturnsNull()
        {
            // Arrange
            var html = "<html><body></body></html>";

            // Act
            var result = _htmlParser.QueryText(html, "h1");

            // Assert
            result.Should().Be(null);
        }

        [Test]
        public void QueryText_ShouldTrimText()
        {
            // Arrange
            var html = "<h1>   Hello   </h1>";

            // Act
            var result = _htmlParser.QueryText(html, "h1");

            // Assert
            result.Should().Be("Hello");
        }
    }
}
