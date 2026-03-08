namespace SimpleECommerceBackend.Domain.Utils;

public static class AgeUtils
{
    public static int Calculate(DateOnly birthDate, DateOnly today)
    {
        var age = today.Year - birthDate.Year;

        if (today < birthDate.AddYears(age)) age--;

        return age;
    }

    public static DateOnly CreateRandomBirthDate(int minAge, int maxAge)
    {
        if (minAge < 0 || maxAge < 0 || minAge > maxAge)
            throw new ArgumentException("Invalid age range.");

        var current = DateOnly.FromDateTime(DateTime.UtcNow);

        var minDate = current.AddYears(-maxAge);
        var maxDate = current.AddYears(-minAge);

        var rangeDays = maxDate.DayNumber - minDate.DayNumber;

        if (rangeDays <= 0)
            return minDate;

        var randomDays = Random.Shared.Next(rangeDays + 1);

        return DateOnly.FromDayNumber(minDate.DayNumber + randomDays);
    }
}