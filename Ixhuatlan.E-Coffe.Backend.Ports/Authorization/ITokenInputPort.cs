using Ixhuatlan.E_Coffe.Backend.Entities.DTOs.Authorization;

namespace Ixhuatlan.E_Coffe.Backend.Ports.Authorization;
public interface ITokenInputPort
{
    TokenRequestDto GenerateToken(string username);
    bool ValidateCredentials(string username, string password);
}