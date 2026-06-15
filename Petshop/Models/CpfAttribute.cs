using System.ComponentModel.DataAnnotations;

namespace Petshop.Models;

public class CpfAttribute : ValidationAttribute
{
    public CpfAttribute()
    {
        ErrorMessage = "CPF inválido.";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string cpf)
        {
            return false;
        }

        var digits = new string(cpf.Where(char.IsDigit).ToArray());
        if (digits.Length != 11)
        {
            return false;
        }

        if (digits.Distinct().Count() == 1)
        {
            return false;
        }

        var numbers = digits.Select(c => c - '0').ToArray();

        var firstDigit = CalculateDigit(numbers, 9, 10);
        var secondDigit = CalculateDigit(numbers, 10, 11);

        return numbers[9] == firstDigit && numbers[10] == secondDigit;
    }

    private static int CalculateDigit(int[] numbers, int length, int factor)
    {
        var sum = 0;
        for (var i = 0; i < length; i++)
        {
            sum += numbers[i] * factor--;
        }

        var result = sum % 11;
        return result < 2 ? 0 : 11 - result;
    }
}