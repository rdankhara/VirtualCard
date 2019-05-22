using System;
using System.Linq;

namespace VirtualCard
{
    public class CardNumberProvider
    {
        private const string ValidCharacters = "0123456789";

        public static string GetNewCardNumber()
        {
            //return Randomizer.CreateRandomizer().GetString(16, ValidCharacters);
            return GenerateRandomNumberString(16);
        }

        public static string GetNewPin()
        {
            //return Randomizer.CreateRandomizer().GetString(16, ValidCharacters);
            return GenerateRandomNumberString(4);
        }

        public static string GetNewCvv()
        {
            return GenerateRandomNumberString(3);
        }
        private static string GenerateRandomNumberString(int number)
        {
            return string.Join("", Enumerable.Range(0, number).Select(x => new Random().Next(0, 9).ToString()));
        }
    }
}
