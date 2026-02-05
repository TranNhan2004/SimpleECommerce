namespace SimpleECommerceBackend.Domain.Utils;

public static class AgeCalculator
{
    public static int Calculate(DateOnly birthDate, DateOnly today)
    {
        var age = today.Year - birthDate.Year;

        if (today < birthDate.AddYears(age)) age--;

        return age;
    }
}