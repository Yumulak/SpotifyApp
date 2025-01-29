public class SimulatedKeyVaultService
{
    private readonly string storageFilePath;
    private readonly byte[] _key = Encoding.UTF8.GetBytes("Your16ByteKeyHere!");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("ThisIsAnIV123456");  // Must be 16 bytes

    public SimulatedKeyVaultService(string encryptionKey)
    {
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpotifyConsolePrototype");
        Directory.CreateDirectory(appDataPath); 
        storageFilePath = Path.Combine(appDataPath, "simulated_key_vault.txt");

        // Ensure the encryption key is 32 bytes for AES-256
        _key = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32).Substring(0, 32));
    }

    public void StoreSecret(string key, string value)
    {
        var encryptedValue = Encrypt(value);
        var entry = $"{key}:{encryptedValue}";

        File.AppendAllLines(storageFilePath, new[] { entry });
    }

    public string RetrieveSecret(string key)
    {
        if (!File.Exists(storageFilePath))
            throw new InvalidOperationException("Storage file not found.");
        var secret = "";
        foreach (var line in File.ReadAllLines(storageFilePath))
        {
            var parts = line.Split(':');
            if (parts.Length == 2 && parts[0] == key)
            {
                secret = Decrypt(parts[1]);
            }
        }
        return secret;
        throw new KeyNotFoundException($"Key '{key}' not found.");
    }

    private string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        aes.IV = IV;
        var iv = aes.IV;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        ms.Write(iv, 0, iv.Length); // Prepend IV to the encrypted data
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    //problem where secret is not returned in Decrypt()
    private string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = new byte[16];
        Array.Copy(fullCipher, iv, iv.Length);
        aes.IV = iv;

        var cipherBytes = new byte[fullCipher.Length - iv.Length];
        Array.Copy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(cipherBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}
