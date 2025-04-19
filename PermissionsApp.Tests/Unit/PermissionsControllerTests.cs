using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PermissionsApp.Application.Commands;
using PermissionsApp.Application.DTOs;
using PermissionsApp.Application.Queries;
using PermissionsApp.Controllers;

namespace PermissionsApp.Tests.Controllers
{
    public class PermissionsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PermissionsController _controller;

        public PermissionsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PermissionsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetPermissions_ReturnsOkResult_WithPermissionsList()
        {
            // Arrange
            var permissions = new List<PermissionDto>
            {
                new PermissionDto 
                { 
                    Id = 1, 
                    EmployeeName = "John", 
                    EmployeeLastName = "Doe", 
                    Date = DateTime.Now, 
                    PermissionTypeId = 1, 
                    PermissionTypeDescription = "Level 1" 
                },
                new PermissionDto 
                { 
                    Id = 2, 
                    EmployeeName = "Jane", 
                    EmployeeLastName = "Smith", 
                    Date = DateTime.Now, 
                    PermissionTypeId = 2, 
                    PermissionTypeDescription = "Level 2" 
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPermissionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(permissions);

            // Act
            var result = await _controller.GetPermissions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResultDto<IEnumerable<PermissionDto>>>(okResult.Value);
            Assert.False(returnValue.IsError);
            Assert.Equal(permissions, returnValue.Data);
        }

        [Fact]
        public async Task GetPermissions_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPermissionsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetPermissions();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var returnValue = Assert.IsType<ResultDto<IEnumerable<PermissionDto>>>(statusCodeResult.Value);
            Assert.True(returnValue.IsError);
            Assert.Equal("Database error", returnValue.ErrorMessage);
        }

        [Fact]
        public async Task GetPermission_ReturnsOkResult_WithPermission_WhenPermissionExists()
        {
            // Arrange
            var permissionId = 1;
            var permission = new PermissionDto 
            { 
                Id = permissionId,
                EmployeeName = "John",
                EmployeeLastName = "Doe",
                Date = DateTime.Now ,
                PermissionTypeId = 1,
                PermissionTypeDescription = "Level 1"
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetPermissionByIdQuery>(q => q.Id == permissionId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(permission);

            // Act
            var result = await _controller.GetPermission(permissionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResultDto<PermissionDto>>(okResult.Value);
            Assert.False(returnValue.IsError);
            Assert.Equal(permission, returnValue.Data);
        }

        [Fact]
        public async Task GetPermission_ReturnsNotFound_WhenPermissionDoesNotExist()
        {
            // Arrange
            var permissionId = 999;
            _mediatorMock.Setup(m => m.Send(It.Is<GetPermissionByIdQuery>(q => q.Id == permissionId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PermissionDto)null);

            // Act
            var result = await _controller.GetPermission(permissionId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResultDto<PermissionDto>>(notFoundResult.Value);
            Assert.True(returnValue.IsError);
            Assert.Contains($"Permission with ID {permissionId} not found", returnValue.ErrorMessage);
        }

        [Fact]
        public async Task RequestPermission_ReturnsCreatedAtAction_WithNewPermission()
        {
            // Arrange
            var requestDto = new RequestPermissionDto
            {
                EmployeeName = "John",
                EmployeeLastName = "Doe",
                PermissionTypeId = 1
            };

            var createdPermission = new PermissionDto
            {
                Id = 1,
                EmployeeName = "John",
                EmployeeLastName = "Doe",
                PermissionTypeId = 1,
                PermissionTypeDescription = "Level 1",
                Date = DateTime.Now
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePermissionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdPermission);

            // Act
            var result = await _controller.CreatePermission(requestDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(PermissionsController.GetPermission), createdAtActionResult.ActionName);
            Assert.Equal(createdPermission.Id, createdAtActionResult.RouteValues["id"]);
            
            var returnValue = Assert.IsType<ResultDto<PermissionDto>>(createdAtActionResult.Value);
            Assert.False(returnValue.IsError);
            Assert.Equal(createdPermission, returnValue.Data);
        }

        [Fact]
        public async Task RequestPermission_ReturnsBadRequest_WhenArgumentExceptionOccurs()
        {
            // Arrange
            var requestDto = new RequestPermissionDto
            {
                EmployeeName = "",  // Invalid data
                EmployeeLastName = "Doe",
                PermissionTypeId = 1
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePermissionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException("Invalid request data"));

            // Act
            var result = await _controller.CreatePermission(requestDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<ResultDto<PermissionDto>>(badRequestResult.Value);
            Assert.True(returnValue.IsError);
            Assert.Equal("Invalid request data", returnValue.ErrorMessage);
        }

        [Fact]
        public async Task UpdatePermission_ReturnsOkResult_WithUpdatedPermission()
        {
           // Arrange
           var permissionId = 1;
           var updateDto = new UpdatePermissionDto
           {
               Id = permissionId,
               EmployeeName = "John",
               EmployeeLastName = "Doe",
               PermissionTypeId = 1 
           };

           var updatedPermission = new PermissionDto
           {
               Id = permissionId,
               EmployeeName = "John",
               EmployeeLastName = "Doe",
               PermissionTypeId = 2,
               Date = DateTime.Now
           };

           _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePermissionCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(updatedPermission);

           // Act
           var result = await _controller.UpdatePermission(permissionId, updateDto);

           // Assert
           var okResult = Assert.IsType<OkObjectResult>(result.Result);
           var returnValue = Assert.IsType<ResultDto<PermissionDto>>(okResult.Value);
           Assert.False(returnValue.IsError);
           Assert.Equal(updatedPermission, returnValue.Data);
           Assert.Equal(2, updatedPermission.PermissionTypeId);
        }

        [Fact]
        public async Task UpdatePermission_ReturnsBadRequest_WhenIdsMismatch()
        {
           // Arrange
           var permissionId = 1;
           var updateDto = new UpdatePermissionDto
           {
               Id = 2,  // Different ID
               EmployeeName = "John",
               EmployeeLastName = "Doe",
               PermissionTypeId = 1 
           };

           // Act
           var result = await _controller.UpdatePermission(permissionId, updateDto);

           // Assert
           var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
           var returnValue = Assert.IsType<ResultDto<PermissionDto>>(badRequestResult.Value);
           Assert.True(returnValue.IsError);
           Assert.Equal("ID in URL does not match ID in request body", returnValue.ErrorMessage);
        }

        [Fact]
        public async Task UpdatePermission_ReturnsNotFound_WhenPermissionDoesNotExist()
        {
           // Arrange
           var permissionId = 999;
           var updateDto = new UpdatePermissionDto
           {
               Id = permissionId,
               EmployeeName = "John",
               EmployeeLastName = "Doe",
               PermissionTypeId = 2,
               Date = DateTime.Now
           };

           _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePermissionCommand>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new KeyNotFoundException($"Permission with ID {permissionId} not found"));

           // Act
           var result = await _controller.UpdatePermission(permissionId, updateDto);

           // Assert
           var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
           var returnValue = Assert.IsType<ResultDto<PermissionDto>>(notFoundResult.Value);
           Assert.True(returnValue.IsError);
           Assert.Equal($"Permission with ID {permissionId} not found", returnValue.ErrorMessage);
        }
    }
}