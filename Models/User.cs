using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankAccount.Validations;

namespace BankAccount.Models {
    public class User {

        [Key]
        public int UserId { get; set; }

        [Required (ErrorMessage = "Enter your first name")]
        [MinLength (2, ErrorMessage = "First name must be at least 2 characters")]
        [RegularExpression (@"^[a-zA-Z]+$", ErrorMessage = "first name must be letters only")]
        public string FirstName { get; set; }

        [Required (ErrorMessage = "Enter your last name")]
        [MinLength (2, ErrorMessage = "Last name must be at least 2 characters")]
        [RegularExpression (@"^[a-zA-Z]+$", ErrorMessage = "Last name must be letters only")]
        public string LastName { get; set; }

        [Required (ErrorMessage = "Enter your email")]
        [EmailAddress (ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [DataType (DataType.Date)]
        [Eighteen]
        public DateTime Birthday { get; set; }

        [DataType (DataType.Password)]
        [Required (ErrorMessage = "Enter a password")]
        [MinLength (8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string Password { get; set; }
        
        public decimal Balance { get; set; } = 0;

        [NotMapped]
        [Compare ("Password", ErrorMessage = "Passwords do not match")]
        [DataType (DataType.Password)]
        public string Confirm { get; set; }

        public List<Transaction> OwnerTransactions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int Age () {
            return DateTime.Now.Year - Birthday.Year;
        }
    }
}