using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;

public static class PasswordHelper
{
    public static string HashPasswordPBKDF2(string senha)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: senha,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32));

        return $"{Convert.ToBase64String(salt)}:{hash}";
    }


    public static string HashPasswordBCrypt(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public static bool VerificarSenha(string senhaDigitada, string senhaHash)
    {
        // Se for BCrypt
        if (senhaHash.StartsWith("$2a$") || senhaHash.StartsWith("$2b$"))
        {
            return BCrypt.Net.BCrypt.Verify(senhaDigitada, senhaHash);
        }

        // Caso contrário, assume PBKDF2
        var partes = senhaHash.Split(':');
        if (partes.Length != 2) return false;

        var salt = Convert.FromBase64String(partes[0]);
        var hashSalvo = partes[1];

        string hashDigitado = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: senhaDigitada,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32));

        return hashDigitado == hashSalvo;
    }
}
