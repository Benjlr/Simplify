
using System;
using System.Collections.Generic;

namespace SimpFinanceService.Models
{

    /// <summary>
    /// This class is a dummy, would be extended with logging operations to contain errors occuring on the server side
    /// </summary>
    public class Logic
    {

        public NumberToWord GenerateWord(decimal totalNumber)
        {
            try
            {
                var result = WordFunction.NumberToWords(totalNumber);
                var nm = new NumberToWord()
                {
                    InputNumber = totalNumber,
                    OutputText = result
                };

                DBOperations.Instance.AddNumberModel(nm);

                return nm;
            }
            catch (Exception ex)
            {
                //logging
                return new NumberToWord();
            }
        }

        public IEnumerable<NumberToWord> GetAll()
        {
            try
            {
                return DBOperations.Instance.GetAllNumberModels();
            }
            catch
            {
                return new List<NumberToWord>();
            }
        }
    }
}