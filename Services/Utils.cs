using System.Security.Cryptography;

namespace Inventory_Management.Services;

public static class Utils
{
    private const char stringDelimeter = ':';
    // It returns the file paths for various files that are used by the application
    public static string GetDirectoryPath()
    {
        return "../../Stock Item";
    }

    public static string GetUsersFilePath()
    {
        return Path.Combine(GetDirectoryPath(), "User.json");
    }

    public static string GetItemsFilePath()
    {
        return Path.Combine(GetDirectoryPath(), "Item.json");
    }
    public static string GetItemsHistoryFilePath()
    {
        return Path.Combine(GetDirectoryPath(), "Record.json");
    }
    // takes a plaintext password and returns a hash of the password, along with a salt and some metadata. It uses the Rfc2898DeriveBytes class from the System.Security.Cryptography namespace to derive a key from the password and salt using the PBKDF2 algorithm.
    public static string HashPassword(string password)
    {
        var saltSize = 16;
        int iterations = 100000;
        var keySize = 32;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA256;

        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);

        return string.Join(
            stringDelimeter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            iterations,
            hashAlgorithm
        );
    }

    /*takes a plaintext password and a hashed password, and returns a boolean 
     * indicating whether the plaintext password matches the original password 
     * that was used to create the hash. It uses the CryptographicOperations.FixedTimeEquals 
     * method to compare the two passwords in a way that is resistant to timing attacks.
     */
    public static bool HashVerification(string password, string hashPassword)
    {
        string[] hashStrings = hashPassword.Split(stringDelimeter);
        byte[] hash = Convert.FromHexString(hashStrings[0]);
        byte[] salt = Convert.FromHexString(hashStrings[1]);
        int iterations = int.Parse(hashStrings[2]);
        HashAlgorithmName hashAlgorithm = new(hashStrings[3]);

        byte[] inputPasswordHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            hashAlgorithm,
            hash.Length
        );

        return CryptographicOperations.FixedTimeEquals(inputPasswordHash, hash);
    }
}
