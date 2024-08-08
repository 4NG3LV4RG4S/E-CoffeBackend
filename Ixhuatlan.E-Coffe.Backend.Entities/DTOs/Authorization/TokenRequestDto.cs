namespace Ixhuatlan.E_Coffe.Backend.Entities.DTOs.Authorization;

public class TokenRequestDto
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
}