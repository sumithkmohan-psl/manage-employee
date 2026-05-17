using ManageEmployee.Models.DTOs;
using System.Text.RegularExpressions;

namespace ManageEmployee.Business.Validators
{
    public static class EmployeeValidator
    {
        public static bool IsEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        public static bool IsValidName(string value)
        {
            return Regex.IsMatch(value, @"^[A-Za-z ]{3,100}$");
        }
        public static bool IsValidSSN(string value)
        {
            return Regex.IsMatch(
                value,
                @"^(?!000|666|9\d\d)\d{3}-(?!00)\d{2}-(?!0000)\d{4}$");
        }
        public static bool IsValidAddress(string value)
        {
            return Regex.IsMatch(value, @"^[A-Za-z0-9\s,.-]{5,200}$");
        }
        public static bool IsValidCity(string value)
        {
            return Regex.IsMatch(value, @"^[A-Za-z\s]{2,100}$");
        }
        public static bool IsValidState(string value)
        {
            return Regex.IsMatch(value, @"^[A-Za-z\s]{2,100}$");
        }
        public static bool IsValidZip(string value)
        {
            return Regex.IsMatch(value, @"^\d{5}(-\d{4})?$");
        }
        public static bool IsValidPhone(string value)
        {
            return Regex.IsMatch(value, @"^\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$");
        }

        public static bool IsValidDate(string input, out DateTime date)
        {
            return DateTime.TryParse(input, out date);
        }

        public static bool IsValidDOB(DateTime date)
        {
            return date >= DateTime.Now.AddYears(-64) && date <= DateTime.Now.AddYears(-22);
        }

        public static bool IsValidJoinDate(DateTime date, DateTime dob)
        {
            return date <= DateTime.Now && date >= dob.AddYears(22) && date <= dob.AddYears(64);
        }

        public static bool IsValidSalary(string input, out decimal salary)
        {
            return decimal.TryParse(input, out salary) && salary > 0 && salary <= 999999999999;
        }

        public static List<string> Validate(EmployeeDto request)
        {
            var errors = new List<string>();

            if (!IsValidName(request.Name))
                errors.Add("Invalid name. Must be 3-100 characters, letters and spaces only.");

            if (!IsValidSSN(request.SSN))
                errors.Add("Invalid SSN. Must be in the format XXX-XX-XXXX.");

            if (!IsValidAddress(request.Address))
                errors.Add("Invalid address. Must be 5-200 characters, letters, numbers, spaces, commas, periods, and hyphens only.");

            if (!IsValidCity(request.City))
                errors.Add("Invalid city. Must be 2-100 characters, letters and spaces only.");

            if (!IsValidState(request.State))
                errors.Add("Invalid state. Must be 2-100 characters, letters and spaces only.");

            if (!IsValidZip(request.Zip))
                errors.Add("Invalid ZIP code. Must be 5 digits or 5 digits followed by a hyphen and 4 digits.");

            if (!IsValidPhone(request.Phone))
                errors.Add("Invalid phone number. Must be in the format (XXX) XXX-XXXX or XXX-XXX-XXXX.");

            if (!IsValidDOB(request.DOB))
                errors.Add("Invalid date of birth. Must be between 22 and 64 years old.");

            if (!IsValidJoinDate(request.JoinDate, request.DOB))
                errors.Add("Invalid join date. Must be after 22 years old and before 64 years old.");

            if (!IsValidTitle(request.Title)) { 
                errors.Add("Invalid title. Must be 2-100 characters, letters and spaces only.");
            }

            if (!IsValidSalary(request.Salary.ToString(), out var salary))
                errors.Add("Invalid salary. Must be a positive number up to 999,999,999,999.");

            return errors;
        }

        public static bool IsValidTitle(string value)
        {
            return Regex.IsMatch(value, @"^[A-Za-z\s]{2,100}$");
        }
    }
}
