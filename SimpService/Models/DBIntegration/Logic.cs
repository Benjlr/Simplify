using System;
using System.Collections.Generic;

namespace SimpService.Models.DBIntegration
{
    public class Logic
    {

        public NumberWord GenerateWord(string totalNumber)
        {
            try
            {
                if (totalNumber.Length > 11) throw new Exception("the number must be 11 digits long at most, including the decimal if present.");
                if (!decimal.TryParse(totalNumber, out var myDecimal))
                {
                    throw new Exception("Please enter an integer or a rational number.");
                }

                var result = WordFunction.NumberToWords(myDecimal);
                var nm = new NumberWord()
                {
                    InputNumber = myDecimal,
                    OutputText = result
                };

                DBOperations.Instance.AddNumberModel(nm);

                return nm;
            }
            catch (Exception ex)
            {
                //logging
                return new NumberWord();
            }
        }

        public IEnumerable<NumberWord> GetAll()
        {
            try
            {
                return DBOperations.Instance.GetAllNumberModels();
            }
            catch
            {
                return new List<NumberWord>();
            }
        }
    }
}