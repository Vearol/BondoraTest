using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Login required")]
        public string LoginNickname { get; set; }

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Wrong password")]
        public string ConfirmPassword { get; set; }

        public RegisterModel()
        {
        }
    }
}
