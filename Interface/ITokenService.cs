using API.Entities;  // Add this line to access AppUser class

namespace API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);  // Method declaration (no body)
    }
}
