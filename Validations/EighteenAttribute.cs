using System;
using System.ComponentModel.DataAnnotations;

namespace BankAccount.Validations {
    public class EighteenAttribute : ValidationAttribute {
        protected override ValidationResult IsValid (object value, ValidationContext validationContext) {

            if (value is DateTime) {
                DateTime check = (DateTime) value;
                if (DateTime.Now.AddYears (-18) > check) {
                    return ValidationResult.Success;
                } else {
                    return new ValidationResult ("Not old enough to be stacking dat paper");
                }
            } else {
                return new ValidationResult ("Please enter a Valid Date.");
            }

        }
    }

}