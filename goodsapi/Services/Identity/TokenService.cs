using Microsoft.AspNetCore.Identity;
using GoodsApi.Extensions;
using GoodsApi.Data.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace GoodsApi.Services.Identity;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(ApplicationUser user, List<IdentityRole<long>> roles)
    {
        var token = user
            .CreateClaims(roles)
            .CreateJwtToken(_configuration);
        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }
}