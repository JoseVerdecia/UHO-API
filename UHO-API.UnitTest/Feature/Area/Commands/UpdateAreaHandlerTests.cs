using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using UHO_API.Core.Entities;
using UHO_API.Core.Enums;
using UHO_API.Core.Interfaces;
using UHO_API.Core.Interfaces.IRepository;
using UHO_API.Features.Area.Commands;
using UHO_API.Shared.Results;

namespace UHO_API.UnitTest.Feature.Area.Commands;
/*
public class UpdateAreaHandlerTests
{
    private readonly Mock<IUnitOfWorks> _mockUow;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IRoleChangesService> _mockRoleChangesService;
    private readonly Mock<ILogger<UpdateAreaHandler>> _mockLogger;
    private readonly Mock<IDbContextTransaction> _mockTransaction;
    private readonly UpdateAreaHandler _handler;

    public UpdateAreaHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWorks>();
        _mockUserManager = MockUserManagerHelper.CreateMockUserManager<ApplicationUser>();
        _mockRoleChangesService = new Mock<IRoleChangesService>();
        _mockLogger = new Mock<ILogger<UpdateAreaHandler>>();
        _mockTransaction = new Mock<IDbContextTransaction>();

        _handler = new UpdateAreaHandler(
            _mockUow.Object,
            _mockUserManager.Object,
            _mockRoleChangesService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUpdatingAreaWithNewJefe()
    {
        // Arrange
        var areaId = 1;
        var oldJefeId = "old-jefe-id";
        var newJefeId = "new-jefe-id";
        var command = new UpdateAreaCommand(areaId, "Área Actualizada", newJefeId);

        var areaToUpdate = new AreaModel { Id = areaId, Nombre = "Área Vieja", JefeAreaId = oldJefeId };
        var newJefe = new ApplicationUser { Id = newJefeId, FullName = "Nuevo Jefe", Email = "nuevo@jefe.com" };

        
        _mockUow.SetupSequence(u => u.Area.Get(It.IsAny<Expression<Func<AreaModel, bool>>>(), null))
            .ReturnsAsync(areaToUpdate) 
            .ReturnsAsync((AreaModel)null); 

        _mockUserManager.Setup(um => um.FindByIdAsync(newJefeId)).ReturnsAsync(newJefe);
        _mockUserManager.Setup(um => um.IsInRoleAsync(newJefe, Roles.UsuarioNormal)).ReturnsAsync(true);

        _mockRoleChangesService.Setup(r => r.DemoteToUsuarioNormalAsync(oldJefeId)).ReturnsAsync(Result.Success());
        _mockRoleChangesService.Setup(r => r.PromoteToJefeAreaAsync(newJefeId)).ReturnsAsync(Result.Success());

        _mockUow.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(_mockTransaction.Object);
        _mockUow.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert 
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Nombre.Should().Be("Área Actualizada");
        result.Value.JefeAreaId.Should().Be(newJefeId);
        result.Value.JefeAreaNombre.Should().Be("Nuevo Jefe");

        // Verificar interacciones
        _mockRoleChangesService.Verify(r => r.DemoteToUsuarioNormalAsync(oldJefeId), Times.Once);
        _mockRoleChangesService.Verify(r => r.PromoteToJefeAreaAsync(newJefeId), Times.Once);
        _mockUow.Verify(u => u.Area.Update(It.Is<AreaModel>(a => a.Nombre == "Área Actualizada")), Times.Once);
        _mockTransaction.Verify(t => t.CommitAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAreaDoesNotExist()
    {
        // Arrange
        var command = new UpdateAreaCommand(99, "Inexistente", null);
        _mockUow.Setup(u => u.Area.Get(It.IsAny<Expression<Func<AreaModel, bool>>>(), null))
            .ReturnsAsync((AreaModel)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Message.Should().Contain("99");

        // Verificar que no se hizo nada (Actualizar/Guardar) en la base de datos.
        _mockUow.Verify(u => u.Area.Update(It.IsAny<AreaModel>()), Times.Never);
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenAreaNameIsAlreadyInUse()
    {
        // Arrange
        var areaId = 1;
        var command = new UpdateAreaCommand(areaId, "Nombre Duplicado", null);
        var areaToUpdate = new AreaModel { Id = areaId, Nombre = "Viejo Nombre" };
        var existingArea = new AreaModel { Id = 2, Nombre = "Nombre Duplicado" };

        _mockUow.SetupSequence(u => u.Area.Get(It.IsAny<Expression<Func<AreaModel, bool>>>(), null))
            .ReturnsAsync(areaToUpdate) // Primera llamada (por ID)
            .ReturnsAsync(existingArea); // Segunda llamada (por nombre duplicado)

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
        result.Error.Message.Should().Contain("Nombre Duplicado");
    }

    [Fact]
    public async Task Handle_ShouldReturnBusinessError_WhenNewJefeIsAlreadyAssigned()
    {
        // Arrange
        var areaId = 1;
        var newJefeId = "jefe-ya-asignado-id";
        var command = new UpdateAreaCommand(areaId, "Nuevo Nombre", newJefeId);
        var areaToUpdate = new AreaModel { Id = areaId };
        var newJefe = new ApplicationUser { Id = newJefeId, FullName = "Jefe Ocupado" };
        var otherArea = new AreaModel { Id = 2, Nombre = "Otra Área", JefeAreaId = newJefeId };

        _mockUow.SetupSequence(u => u.Area.Get(It.IsAny<Expression<Func<AreaModel, bool>>>(), null))
            .ReturnsAsync(areaToUpdate) // Por ID
            .ReturnsAsync((AreaModel)null) // Por nombre duplicado
            .ReturnsAsync(otherArea); // Por jefe asignado

        _mockUserManager.Setup(um => um.FindByIdAsync(newJefeId)).ReturnsAsync(newJefe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Business);
        result.Error.Message.Should().Contain("Jefe Ocupado");
        result.Error.Message.Should().Contain("Otra Área");
    }

    [Fact]
    public async Task Handle_ShouldRollbackTransaction_WhenPromotionFails()
    {
        // Arrange
        var areaId = 1;
        var newJefeId = "new-jefe-id";
        var command = new UpdateAreaCommand(areaId, "Área Fallida", newJefeId);
        var areaToUpdate = new AreaModel { Id = areaId };
        var newJefe = new ApplicationUser { Id = newJefeId };

        _mockUow.SetupSequence(u => u.Area.Get(It.IsAny<Expression<Func<AreaModel, bool>>>(), null))
            .ReturnsAsync(areaToUpdate)
            .ReturnsAsync((AreaModel)null);

        _mockUserManager.Setup(um => um.FindByIdAsync(newJefeId)).ReturnsAsync(newJefe);
        _mockUserManager.Setup(um => um.IsInRoleAsync(newJefe, Roles.UsuarioNormal)).ReturnsAsync(true);

        // La promoción falla
        _mockRoleChangesService.Setup(r => r.PromoteToJefeAreaAsync(newJefeId))
            .ReturnsAsync(Result.Failure(Error.Failure("PromotionFailed", "Error de promoción")));

        _mockUow.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(_mockTransaction.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Failure);

        // Verificación crucial: la transacción se hizo rollback
        _mockTransaction.Verify(t => t.RollbackAsync(CancellationToken.None), Times.Once);
        _mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
*/