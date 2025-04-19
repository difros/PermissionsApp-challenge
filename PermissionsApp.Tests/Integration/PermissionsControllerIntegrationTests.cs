using Newtonsoft.Json;
using PermissionsApp.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace PermissionsApp.Tests.Integration
{
    public class PermissionsControllerIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetPermissions_ReturnsAllPermissions()
        {
            // Act
            var response = await _client.GetAsync("/api/permissions");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultDto<IEnumerable<PermissionDto>>>(content);
            
            Assert.False(result.IsError);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetPermission_ReturnsPermission_WhenPermissionExists()
        {
            // Act
            var response = await _client.GetAsync("/api/permissions/1");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultDto<PermissionDto>>(content);
            
            Assert.False(result.IsError);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("Test", result.Data.EmployeeName);
            Assert.Equal("User", result.Data.EmployeeLastName);
            Assert.Equal(1, result.Data.PermissionTypeId);
        }

        [Fact]
        public async Task GetPermission_ReturnsNotFound_WhenPermissionDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync("/api/permissions/999");
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultDto<PermissionDto>>(content);
            
            Assert.True(result.IsError);
            Assert.Contains("not found", result.ErrorMessage);
        }

        [Fact]
        public async Task CreatePermission_CreatesNewPermission_WhenDataIsValid()
        {
            // Arrange
            var newPermission = new RequestPermissionDto
            {
                EmployeeName = "New",
                EmployeeLastName = "Employee",
                PermissionTypeId = 2
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(newPermission),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/permissions/request", jsonContent);
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultDto<PermissionDto>>(content);
            
            Assert.False(result.IsError);
            Assert.NotNull(result.Data);
            Assert.Equal("New", result.Data.EmployeeName);
            Assert.Equal("Employee", result.Data.EmployeeLastName);
            Assert.Equal(2, result.Data.PermissionTypeId);

            // Verify the permission was actually created in the database
            var getAllResponse = await _client.GetAsync("/api/permissions");
            var getAllContent = await getAllResponse.Content.ReadAsStringAsync();
            var getAllResult = JsonConvert.DeserializeObject<ResultDto<IEnumerable<PermissionDto>>>(getAllContent);
            
            Assert.Equal(3, getAllResult.Data.Count());
        }

        [Fact]
        public async Task UpdatePermission_UpdatesExistingPermission_WhenDataIsValid()
        {
            // Arrange
            var updatePermission = new UpdatePermissionDto
            {
                Id = 1,
                EmployeeName = "Updated",
                EmployeeLastName = "User",
                PermissionTypeId = 1
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(updatePermission),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PutAsync("/api/permissions/1", jsonContent);
            
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultDto<PermissionDto>>(content);
            
            Assert.False(result.IsError);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("Updated", result.Data.EmployeeName);
            Assert.Equal("User", result.Data.EmployeeLastName);
            Assert.Equal(1, result.Data.PermissionTypeId);

            // Verify the permission was actually updated in the database
            var getResponse = await _client.GetAsync("/api/permissions/1");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var getResult = JsonConvert.DeserializeObject<ResultDto<PermissionDto>>(getContent);
            
            Assert.Equal("Updated", getResult.Data.EmployeeName);
            Assert.Equal(1, getResult.Data.PermissionTypeId);
        }

        [Fact]
        public async Task UpdatePermission_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            var updatePermission = new UpdatePermissionDto
            {
                Id = 2,  // Different ID
                EmployeeName = "Updated",
                EmployeeLastName = "User",
                PermissionTypeId = 1
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(updatePermission),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PutAsync("/api/permissions/1", jsonContent);
            
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultDto<PermissionDto>>(content);
            
            Assert.True(result.IsError);
            Assert.Equal("ID in URL does not match ID in request body", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdatePermission_ReturnsNotFound_WhenPermissionDoesNotExist()
        {
            // Arrange
            var updatePermission = new UpdatePermissionDto
            {
                Id = 999,
                EmployeeName = "Updated",
                EmployeeLastName = "User",
                PermissionTypeId = 1
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(updatePermission),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PutAsync("/api/permissions/999", jsonContent);
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultDto<PermissionDto>>(content);
            
            Assert.True(result.IsError);
            Assert.Contains("not found", result.ErrorMessage);
        }
    }
}