using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
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
public class CreateAreaHandlerTests
{
    private readonly Mock<IUnitOfWorks> _mockUow;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IRoleChangesService> _mockRoleChangesService;
    private readonly Mock<ILogger<CreateAreaHandler>> _mockLogger;
    private readonly CreateAreaHandler _handler;


    public CreateAreaHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWorks>();
        _mockUserManager = MockUserManagerHelper.CreateMockUserManager<ApplicationUser>();
        _mockRoleChangesService = new Mock<IRoleChangesService>();
        _mockLogger = new Mock<ILogger<CreateAreaHandler>>();

        _handler = new CreateAreaHandler(
            _mockUow.Object,
            _mockUserManager.Object,
            _mockRoleChangesService.Object
        );
    }

     [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAreaIsCreatedWithJefe()
    {
        // Arrange
        var command = new CreateAreaCommand("Pre-Grado", "BC3ED166-0A13-4C52-A2E3-5350A9E54C51");
        var fakeJefe = new ApplicationUser { Id = "BC3ED166-0A13-4C52-A2E3-5350A9E54C51", FullName = "Jefe de Área", Email = "jefe@ejemplo.com" };

        _mockUow.Setup(u => u.Area.Get(It.IsAny<Expression<Func<AreaModel, bool>>>(), null))
            .ReturnsAsync((AreaModel)null);

        _mockUserManager.Setup(um => um.FindByIdAsync(fakeJefe.Id)).ReturnsAsync(fakeJefe);

        _mockRoleChangesService.Setup(r => r.PromoteToJefeAreaAsync(fakeJefe.Id))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert 
        result.IsSuccess.Should().BeTrue("porque el área se creó exitosamente");
        result.Value.Should().NotBeNull("porque se debe devolver la información del área creada");

      
        result.Value.Nombre.Should().Be("Pre-Grado");
        result.Value.JefeAreaId.Should().Be(fakeJefe.Id);
        result.Value.JefeAreaNombre.Should().Be("Jefe de Área");
        result.Value.JefeAreaEmail.Should().Be("jefe@ejemplo.com");

       
        _mockUow.Verify(u => u.Area.Add(It.IsAny<AreaModel>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        _mockRoleChangesService.Verify(r => r.PromoteToJefeAreaAsync(fakeJefe.Id), Times.Once);
    }
    

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenAreaNameAlreadyExists()
    {
        // Arrange
        var command = new CreateAreaCommand("Recursos Humanos", null);
        var existingArea = new AreaModel { Nombre = "Recursos Humanos" };

        _mockUow.Setup(u => u.Area.Get(It.IsAny<Expression<Func<AreaModel, bool>>>(), null))
                .ReturnsAsync(existingArea);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeNull();
        
        result.Errors.Type.Should().Be(ErrorType.Conflict);
        result.Errors.Message.Should().Contain("Nombre");
        result.Errors.Message.Should().Contain("Recursos Humanos");


        _mockUow.Verify(u => u.Area.Add(It.IsAny<AreaModel>()), Times.Never);
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
*/

