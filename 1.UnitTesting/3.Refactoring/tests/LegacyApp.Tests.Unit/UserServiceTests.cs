using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace LegacyApp.Tests.Unit;

public class UserServiceTests
{
    private readonly IClock _clock = Substitute.For<IClock>();
    private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
    private readonly IUserCreditServiceClientFactory _userCreditServiceFactory =
        Substitute.For<IUserCreditServiceClientFactory>();

    private readonly IUserDataAccessAdapter _userDataAccess = Substitute.For<IUserDataAccessAdapter>();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_clock, _clientRepository, _userCreditServiceFactory, _userDataAccess);
    }
    
    [Fact]
    public void AddUser_ShouldNotCreateUser_WhenFirstNameIsEmpty()
    {
        // Arrange
        var firstName = string.Empty;
        var surName = "Chapsas";
        var email = "nick@dometrain.com";
        var dob = new DateTime(1993, 1, 1);
        var clientId = 60;
        _clock.Now.Returns(new DateTime(2023, 1, 1));

        // Act
        var result = _sut.AddUser(firstName, surName, email, dob, clientId);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void AddUser_ShouldNotCreateUser_WhenLastNameIsEmpty()
    {
        // Arrange
        var firstName = "Nick";
        var surName = string.Empty;
        var email = "nick@dometrain.com";
        var dob = new DateTime(1993, 1, 1);
        var clientId = 60;
        _clock.Now.Returns(new DateTime(2023, 1, 1));

        // Act
        var result = _sut.AddUser(firstName, surName, email, dob, clientId);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void AddUser_ShouldNotCreateUser_WhenEmailIsInvalid()
    {
        // Arrange
        var firstName = "Nick";
        var surName = "Chapsas";
        var email = "nickdometraincom";
        var dob = new DateTime(1993, 1, 1);
        var clientId = 60;
        _clock.Now.Returns(new DateTime(2023, 1, 1));

        // Act
        var result = _sut.AddUser(firstName, surName, email, dob, clientId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AddUser_ShouldNotCreateUser_WhenUserIsUnder21()
    {
        // Arrange
        var firstName = "Nick";
        var surName = "Chapsas";
        var email = "nick@dometrain.com";
        var dob = new DateTime(2004, 1, 1);
        var clientId = 60;
        _clock.Now.Returns(new DateTime(2023, 1, 1));
        
        // Act
        var result = _sut.AddUser(firstName, surName, email, dob, clientId);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AddUser_ShouldNotCreateUser_WhenUserHasCreditLimitAndLimitIsLessThan500()
    {
        // Arrange
        var firstName = "Nick";
        var surName = "Chapsas";
        var email = "nick@dometrain.com";
        var dob = new DateTime(2000, 1, 1);
        var clientId = 60;
        _clock.Now.Returns(new DateTime(2023, 1, 1));

        var client = new Client
        {
            Id = clientId,
            Name = "Nick Chapsas",
            ClientStatus = ClientStatus.Gold
        };

        _clientRepository.GetById(clientId).Returns(client);

        var userServiceClient = Substitute.For<IUserCreditService>();
        userServiceClient.GetCreditLimit(firstName, surName, dob).Returns(499);
        _userCreditServiceFactory.CreateClient().Returns(userServiceClient);

        // Act
        var result = _sut.AddUser(firstName, surName, email, dob, clientId);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AddUser_ShouldCreateUser_WhenUserDetailsAreValid()
    {
        // Arrange
        var firstName = "Nick";
        var surName = "Chapsas";
        var email = "nick@dometrain.com";
        var dob = new DateTime(2000, 1, 1);
        var clientId = 60;
        _clock.Now.Returns(new DateTime(2023, 1, 1));

        var client = new Client
        {
            Id = clientId,
            Name = "Nick Chapsas",
            ClientStatus = ClientStatus.Gold
        };

        _clientRepository.GetById(clientId).Returns(client);

        var userServiceClient = Substitute.For<IUserCreditService>();
        userServiceClient.GetCreditLimit(firstName, surName, dob).Returns(500);
        _userCreditServiceFactory.CreateClient().Returns(userServiceClient);
        
        // Act
        var result = _sut.AddUser(firstName, surName, email, dob, clientId);
        
        // Assert
        result.Should().BeTrue();
        _userDataAccess.Received(1).AddUser(Arg.Any<User>());
    }
}
