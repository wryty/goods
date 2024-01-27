using Microsoft.AspNetCore.Identity;
using GoodsApi.Data.Entities;

namespace GoodsApi.Services.Identity;


public interface ITokenService
{
    string CreateToken(ApplicationUser user, List<IdentityRole<long>> role);
}