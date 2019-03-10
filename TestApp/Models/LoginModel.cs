using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Require login:")]
        public string LoginNickname { get; set; }

        [Required(ErrorMessage = "Require password:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public LoginModel(string loginNickname, string password)
        {
            LoginNickname = loginNickname;
            Password = password;
        }

        public LoginModel()
        {
        }
    }
}
