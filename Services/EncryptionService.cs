using System.Security.Cryptography;
using System.Text;

namespace BriefingApp.Services;

public class EncryptionService {
    
    private readonly byte[] _key;

    public EncryptionService(IConfiguration configuration) {
        
        var keyString = configuration["Encryption:Key"] ?? throw new Exception("Encryption key not found");
        _key = SHA256.HashData(Encoding.UTF8.GetBytes(keyString));

    }

    public string Encrypt(string plainText) {
        
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var result = new byte[aes.IV.Length + encryptedBytes.Length];
        aes.IV.CopyTo(result, 0);
        encryptedBytes.CopyTo(result, aes.IV.Length);

        return Convert.ToBase64String(result);

    }

    public string Decrypt(string encryptedText) {
        
        var allBytes = Convert.FromBase64String(encryptedText);

        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = allBytes[..aes.IV.Length];
        var encryptedBytes = allBytes[aes.IV.Length..];
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);

    }

}