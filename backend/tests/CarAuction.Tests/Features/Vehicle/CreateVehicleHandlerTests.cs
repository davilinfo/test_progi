using CarAuction.Api.Features.Vehicle.Create;
using CarAuction.Api.Features.Vehicle.Repository;
using CarAuction.Api.Shared.Entity;
using CarAuction.Api.Shared.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CarAuction.Tests.Features.Vehicle;

public class CreateVehicleHandlerTests
{
    private readonly Mock<IVehicleRepository> _repoMock;
    private readonly Mock<ILogger<CreateVehicleHandler>> _loggerMock;
    private readonly CreateVehicleHandler _handler;

    public CreateVehicleHandlerTests()
    {
        _repoMock = new Mock<IVehicleRepository>();
        _loggerMock = new Mock<ILogger<CreateVehicleHandler>>();
        _handler = new CreateVehicleHandler(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAndReturnsVehicle()
    {
        // Arrange
        var command = new CreateVehicleCommand("ABC-123", VehicleType.Common, 2022, "Toyota Corolla", null);
        var createdVehicle = new Api.Shared.Entity.Vehicle
        {
            Id = Guid.NewGuid(),
            Plate = command.Plate,
            Type = command.Type,
            Year = command.Year,
            Model = command.Model,
            Photo = command.Photo
        };

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Api.Shared.Entity.Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdVehicle);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(createdVehicle.Id);
        result.Plate.Should().Be("ABC-123");
        result.Type.Should().Be(VehicleType.Common);
        result.Year.Should().Be(2022);
        result.Model.Should().Be("Toyota Corolla");

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Api.Shared.Entity.Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LuxuryVehicle_SetsTypeCorrectly()
    {
        // Arrange
        var command = new CreateVehicleCommand("XYZ-999", VehicleType.Luxury, 2023, "BMW 7 Series", "photo.jpg");
        var createdVehicle = new Api.Shared.Entity.Vehicle
        {
            Id = Guid.NewGuid(),
            Plate = command.Plate,
            Type = command.Type,
            Year = command.Year,
            Model = command.Model,
            Photo = command.Photo
        };

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Api.Shared.Entity.Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdVehicle);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Type.Should().Be(VehicleType.Luxury);
        result.Photo.Should().Be("photo.jpg");
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectData()
    {
        // Arrange
        var command = new CreateVehicleCommand("TEST-01", VehicleType.Common, 2020, "Honda Civic", null);
        Api.Shared.Entity.Vehicle? capturedVehicle = null;

        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Api.Shared.Entity.Vehicle>(), It.IsAny<CancellationToken>()))
            .Callback<Api.Shared.Entity.Vehicle, CancellationToken>((v, _) => capturedVehicle = v)
            .ReturnsAsync((Api.Shared.Entity.Vehicle v, CancellationToken _) =>
            {
                v.Id = Guid.NewGuid();
                return v;
            });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedVehicle.Should().NotBeNull();
        capturedVehicle!.Plate.Should().Be("TEST-01");
        capturedVehicle.Type.Should().Be(VehicleType.Common);
        capturedVehicle.Year.Should().Be(2020);
        capturedVehicle.Model.Should().Be("Honda Civic");
    }
}
