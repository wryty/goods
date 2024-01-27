using System.ComponentModel.DataAnnotations;
namespace GoodsApi.Models.Identity;

public class RegisterRequest
{
    [Required]
    [Display(Name = "UserName")]
    public string UserName { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;
}