using HridhayConnect_API.Models;

namespace HridhayConnect_API.Infra
{
    public class ValidationService
    {
        public CommonViewModel ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = $"Please Enter {fieldName}"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }
        public CommonViewModel ValidateRequired_Decimal(decimal? value, string fieldName)
        {
            if (value == null || value<= 0)
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = $"Please Enter {fieldName}"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }
        public CommonViewModel ValidateMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Please Enter Mobile Number"
                };
            }
            if (!mobile.All(char.IsDigit))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Enter only digits"
                };
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(mobile, @"^[0-9]{10}$"))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Mobile must be 10 digits"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }

        public CommonViewModel ValidatePincode(string pincode)
        {
            if (string.IsNullOrWhiteSpace(pincode))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Please Enter Pincode"
                };
            }

            if (!pincode.All(char.IsDigit))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Pincode must contain only digits"
                };
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(pincode, @"^[0-9]{6}$"))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Pincode must be 6 digits"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }

        public CommonViewModel ValidateEmail(string email)
        {
            // 1. Required check
            if (string.IsNullOrWhiteSpace(email))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Please Enter Email"
                };
            }

            // 2. Trim + normalize
            email = email.Trim().ToLower();

            // 3. Strong format validation (fix for admin@1 issue)
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailRegex))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Please Enter Valid Email"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }

        public CommonViewModel ValidateDropdown_Long(long? value, string fieldName)
        {
            if (value <= 0)
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = $"Please Select {fieldName}"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }
        public CommonViewModel ValidateDropdown_String(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = $"Please Select {fieldName}"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }

        public CommonViewModel ValidateAlphaNumeric(string value, string fieldName)
        {

            if (string.IsNullOrWhiteSpace(value))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = $"Please Enter {fieldName}"
                };
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9]+$"))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = $"{fieldName} must be alphanumeric (only letters and numbers)"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }

        public CommonViewModel ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Please Enter Password"
                };
            }

            // Min 6 chars, 1 letter + 1 number
            var regex = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$";

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, regex))
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Password must be at least 6 characters and contain letters and numbers"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }

        public CommonViewModel ValidateConfirmPassword(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return new CommonViewModel
                {
                    IsSuccess = false,
                    StatusCode = ResponseStatusCode.Error,
                    Message = "Passwords do not match"
                };
            }

            return new CommonViewModel { IsSuccess = true };
        }
    }

}
