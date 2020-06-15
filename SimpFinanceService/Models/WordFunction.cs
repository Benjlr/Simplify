using System;

namespace SimpFinanceService.Models
{
    public class WordFunction
    {
        /// <summary>
        /// takes a decimal and returns an english language string that represents the quantity
        /// </summary>
        /// <param name="totalNumber"></param>
        /// <returns></returns>
        public static string NumberToWords(decimal totalNumber)
        {

            ///
            /// https://stackoverflow.com/questions/2729752/converting-numbers-in-to-words-c-sharp
            /// 
            int number = 0;
            if (totalNumber > 0) number = (int)Math.Floor(totalNumber);
            else number = (int)Math.Ceiling(totalNumber);


            if (totalNumber == 0) return "zero";
            if (number < 0) return "minus " + NumberToWords(Math.Abs(totalNumber));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            ///
            /// My addition for decimals
            /// 
            var decimalPart = totalNumber - Math.Truncate(totalNumber);
            if (decimalPart == 0) return words;
            else
            {
                words += " point";

                while (decimalPart > 0)
                {
                    decimalPart = decimalPart * 10;
                    var newnumber = Math.Floor(decimalPart);
                    words += " " + NumberToWords(newnumber);
                    decimalPart = decimalPart - Math.Truncate(decimalPart);
                }
            }

            return words;
        }
    }
}
