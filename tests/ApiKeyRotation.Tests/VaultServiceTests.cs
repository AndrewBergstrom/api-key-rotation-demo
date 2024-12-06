using System;
using VaultSharp;
using VaultSharp.V1;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines.KeyValue.V2;
using Moq;
using Microsoft.Extensions.Logging;
using Xunit;
using ApiKeyRotation.Services;

namespace ApiKeyRotation.Tests
{
    public class VaultServiceTests
    {
        [Fact]
        public void RotateKey_ValidKeyId_ReturnsNewApiKey()
        {
            // Arrange
            var mockVaultClient = new Mock<IVaultClient>();
            var mockLogger = new Mock<ILogger<VaultService>>();

            // Mock WriteSecretAsync
            mockVaultClient
                .Setup(v => v.V1.Secrets.KeyValue.V2.WriteSecretAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<int?>(), // Correct type
                    It.IsAny<string>()))
                .ReturnsAsync(new Secret<CurrentSecretMetadata> { Data = null }); // Correct type

            var vaultService = new VaultService(mockVaultClient.Object, mockLogger.Object);
            var keyId = "test-key";

            // Act
            var result = vaultService.RotateKey(keyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(keyId, result.Id);
            Assert.False(string.IsNullOrEmpty(result.Key));
        }

        [Fact]
        public void RotateKey_NullOrEmptyKeyId_ThrowsArgumentException()
        {
            // Arrange
            var mockVaultClient = new Mock<IVaultClient>();
            var mockLogger = new Mock<ILogger<VaultService>>();
            var vaultService = new VaultService(mockVaultClient.Object, mockLogger.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => vaultService.RotateKey((string)null!));
            Assert.Throws<ArgumentException>(() => vaultService.RotateKey(string.Empty));
        }
    }
}
