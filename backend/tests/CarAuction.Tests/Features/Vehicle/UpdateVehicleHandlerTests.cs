using CarAuction.Api.Features.Vehicle.Repository;
using CarAuction.Api.Features.Vehicle.Update;
using CarAuction.Api.Shared.Entity;
using CarAuction.Api.Shared.Enums;
using CarAuction.Api.Shared.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarAuction.Tests.Features.Vehicle;

public class UpdateVehicleHandlerTests
{
    private readonly Mock<IVehicleRepository> _repoMock;
    private readonly Mock<ILogger<UpdateVehicleHandler>> _loggerMock;
    private readonly UpdateVehicleHandler _handler;

    public UpdateVehicleHandlerTests()
    {
        _repoMock = new Mock<IVehicleRepository>();
        _loggerMock = new Mock<ILogger<UpdateVehicleHandler>>();
        _handler = new UpdateVehicleHandler(_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoActiveAuction_UpdatesVehicle()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var existing = new Api.Shared.Entity.Vehicle
        {
            Id = vehicleId, Plate = "OLD-001", Type = VehicleType.Common, Year = 2019, Model = "Old Model"
        };
        var command = new UpdateVehicleCommand(vehicleId, "NEW-001", VehicleType.Luxury, 2023, "New Model", null);

        _repoMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repoMock.Setup(r => r.HasActiveAuctionAsync(vehicleId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Api.Shared.Entity.Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Api.Shared.Entity.Vehicle v, CancellationToken _) => v);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Plate.Should().Be("NEW-001");
        result.Type.Should().Be(VehicleType.Luxury);
        result.Model.Should().Be("New Model");
    }

    [Fact]
    public async Task Handle_WithActiveAuction_ThrowsDomainException()
    {
        // Arrange
        var vehicleId = Guid.NewGuid();
        var existing = new Api.Shared.Entity.Vehicle
        {
            Id = vehicleId, Plate = "ABC-001", Type = VehicleType.Common, Year = 2020, Model = "Test"
        };
        var command = new UpdateVehicleCommand(vehicleId, "NEW-001", VehicleType.Common, 2020, "Test", null);

        _repoMock.Setup(r => r.GetByIdAsync(vehicleId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repoMock.Setup(r => r.HasActiveAuctionAsync(vehicleId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*active auction*");
    }

    [Fact]
    public async Task Handle_VehicleNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new UpdateVehicleCommand(Guid.NewGuid(), "NEW-001", VehicleType.Common, 2020, "Test", null);
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Api.Shared.Entity.Vehicle?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
