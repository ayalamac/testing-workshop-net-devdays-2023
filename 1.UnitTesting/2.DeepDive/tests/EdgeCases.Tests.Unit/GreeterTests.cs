using FluentAssertions;
using NSubstitute;
using Xunit;

namespace EdgeCases.Tests.Unit;

public class GreeterTests
{
    [Fact]
    public void GenerateGreetText_ShouldReturnGoodMorning_WhenItsMorning()
    {
        // Arrange
        IClock clock = Substitute.For<IClock>();
        clock.Now.Returns(new DateTime(2023, 1, 1, 10, 0, 0));
        var sut = new Greeter(clock);

        // Act
        var result = sut.GenerateGreetText();

        // Assert
        result.Should().Be("Good morning");
    }
    
    [Fact]
    public void GenerateGreetText_ShouldReturnGoodAfternoon_WhenItsAfternoon()
    {
        // Arrange
        IClock clock = Substitute.For<IClock>();
        clock.Now.Returns(new DateTime(2023, 1, 1, 14, 0, 0));
        var sut = new Greeter(clock);

        // Act
        var result = sut.GenerateGreetText();

        // Assert
        result.Should().Be("Good afternoon");
    }
    
    [Fact]
    public void GenerateGreetText_ShouldReturnGoodEvening_WhenItsEvening()
    {
        // Arrange
        IClock clock = Substitute.For<IClock>();
        clock.Now.Returns(new DateTime(2023, 1, 1, 20, 0, 0));
        var sut = new Greeter(clock);

        // Act
        var result = sut.GenerateGreetText();

        // Assert
        result.Should().Be("Good evening");
    }
}
