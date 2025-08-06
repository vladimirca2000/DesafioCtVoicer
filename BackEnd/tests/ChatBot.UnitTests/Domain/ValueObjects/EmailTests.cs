using System;
using ChatBot.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Domain.ValueObjects
{
    public class EmailTests
    {
        [Theory]
        [InlineData("test@email.com")]
        [InlineData("user.name@domain.co.uk")]
        [InlineData("test+tag@example.org")]
        public void Create_Should_Return_Valid_Email_When_Format_Is_Correct(string validEmail)
        {
            // Act
            var email = Email.Create(validEmail);

            // Assert
            email.Value.ShouldBe(validEmail.ToLowerInvariant());
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-email")]
        [InlineData("@domain.com")]
        [InlineData("user@")]
        public void Create_Should_Throw_ArgumentException_When_Format_Is_Invalid(string invalidEmail)
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => Email.Create(invalidEmail));
        }

        [Fact]
        public void Create_Should_Normalize_Email_To_Lowercase()
        {
            // Arrange
            var upperCaseEmail = "TEST@EMAIL.COM";

            // Act
            var email = Email.Create(upperCaseEmail);

            // Assert
            email.Value.ShouldBe("test@email.com");
        }

        [Fact]
        public void ImplicitOperator_Should_Convert_Email_To_String()
        {
            // Arrange
            var email = Email.Create("test@email.com");

            // Act
            string emailString = email;

            // Assert
            emailString.ShouldBe("test@email.com");
        }

        [Fact]
        public void ExplicitOperator_Should_Convert_String_To_Email()
        {
            // Arrange
            var emailString = "test@email.com";

            // Act
            var email = (Email)emailString;

            // Assert
            email.Value.ShouldBe("test@email.com");
        }

        [Fact]
        public void ToString_Should_Return_Email_Value()
        {
            // Arrange
            var email = Email.Create("test@email.com");

            // Act
            var result = email.ToString();

            // Assert
            result.ShouldBe("test@email.com");
        }

        [Fact]
        public void Equals_Should_Return_True_For_Same_Email_Values()
        {
            // Arrange
            var email1 = Email.Create("test@email.com");
            var email2 = Email.Create("TEST@EMAIL.COM");

            // Act & Assert
            email1.ShouldBe(email2);
        }
    }
}