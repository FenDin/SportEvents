namespace SportEvents.Web.Controllers
{
    public class Utils
    {
        //пример:др уже был в этом году
        //др:01.02.2000, сегодня: 10.03.2026
        //    2026-2000=26
        //    проверка: 01.02.2000 > 10.03.(2026-26) - ответ:нет, значит возраст не уменьшается
        private static int CalcAge(DateTime birthDate, DateTime today)
        {
            int age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age).Date)
                age--;

            return age;
        }
    }
}
