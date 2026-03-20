using Microsoft.Extensions.Configuration;

using BriefingApp.Services;

namespace BriefingApp.Tests;

public class EncryptionServiceTests {
    
    private readonly EncryptionService _encryptionService;

    public EncryptionServiceTests() {
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                { "Encryption:Key", "test-key-for-unit-tests-32chars!" }
            })
            .Build();

        _encryptionService = new EncryptionService(configuration);

    }

    [Fact]
    public void EncryptDecrypt_ShouldReturnSameString() {
        
        string startString = "This is 1 encryption Test!";
        string encryptedString = _encryptionService.Encrypt(startString);
        string endString = _encryptionService.Decrypt(encryptedString);
        Assert.Equal(startString, endString);

    }

    [Fact]
    public void Encrypt_Twice_ShouldReturnDifferentString() {
        
        string testString = "This is 1 encryption Test!";
        string firstEncryptedString = _encryptionService.Encrypt(testString);
        string secondEncryptedString = _encryptionService.Encrypt(testString);
        Assert.NotEqual(firstEncryptedString, secondEncryptedString);

    }

}