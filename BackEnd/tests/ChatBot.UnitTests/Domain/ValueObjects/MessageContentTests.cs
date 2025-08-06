using System;
using ChatBot.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Domain.ValueObjects
{
    public class MessageContentTests
    {
        [Theory]
        [InlineData("Mensagem válida")]
        [InlineData("A")]
        [InlineData("Mensagem com números 123 e símbolos !@#")]
        public void Create_Should_Return_Valid_MessageContent_When_Content_Is_Valid(string validContent)
        {
            // Act
            var messageContent = MessageContent.Create(validContent);

            // Assert
            messageContent.Value.ShouldBe(validContent);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_Should_Throw_ArgumentException_When_Content_Is_Invalid(string invalidContent)
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => MessageContent.Create(invalidContent));
        }

        [Fact]
        public void Create_Should_Accept_Content_With_Whitespace()
        {
            // Arrange
            var contentWithWhitespace = "  Mensagem com espaços  ";

            // Act
            var messageContent = MessageContent.Create(contentWithWhitespace);

            // Assert
            messageContent.Value.ShouldBe("  Mensagem com espaços  ");
        }

        [Fact]
        public void ImplicitOperator_Should_Convert_MessageContent_To_String()
        {
            // Arrange
            var messageContent = MessageContent.Create("Test message");

            // Act
            string contentString = messageContent;

            // Assert
            contentString.ShouldBe("Test message");
        }

        [Fact]
        public void ToString_Should_Return_Content_Value()
        {
            // Arrange
            var messageContent = MessageContent.Create("Test message");

            // Act
            var result = messageContent.ToString();

            // Assert
            result.ShouldBe("Test message");
        }

        [Fact]
        public void Equals_Should_Return_True_For_Same_Content_Values()
        {
            // Arrange
            var content1 = MessageContent.Create("Same message");
            var content2 = MessageContent.Create("Same message");

            // Act & Assert
            content1.ShouldBe(content2);
        }
    }
}