using Iskra.Application.Abstractions.Security;

namespace Iskra.Infrastructure.Shared.Security;

internal sealed class PasswordHasher : IPasswordHasher
{
    private const int _workFactor = 12;

    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor: _workFactor);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    }
}