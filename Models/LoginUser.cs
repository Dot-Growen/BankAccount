using System.ComponentModel.DataAnnotations;

namespace BankAccount.Models {
    public class LoginUser {

        [Required (ErrorMessage = "Enter your email")]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Required (ErrorMessage = "Enter a password")]
        [DataType (DataType.Password)]
        public string LoginPassword { get; set; }
    }
}