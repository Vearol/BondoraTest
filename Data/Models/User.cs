using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; private set; }
        public string LoginNickname { get; private set; }
        public string Password { get; private set; }
        public int ExtraPoints { get; set; }

        public User(string loginNickname, string password)
        {
            LoginNickname = loginNickname;
            Password = password;
        }
    }
}
