using System.Collections.Generic;
using System.Linq;

namespace SimpFinanceService.Models
{
    public class DBOperations
    {
        private static DBOperations _instance;
        public static DBOperations Instance => _instance ?? (_instance = new DBOperations());

        private DBOperations()
        {

        }


        public void AddNumberModel(NumberToWord nm)
        {
            using (var ent = SimplifyDBHelper.DBEntities)
            {
                ent.NumberToWords.Add(nm);
                ent.SaveChanges();
            }
        }

        public IEnumerable<NumberToWord> GetAllNumberModels()
        {
            using (var ent = SimplifyDBHelper.DBEntities)
            {
                return ent.NumberToWords.ToList();
            }
        }
    }
}