using System.Collections.Generic;
using System.Linq;

namespace SimpFinanceService.Models
{

    /// <summary>
    /// Singleton to assist with DB operations
    /// </summary>
    public class DBOperations
    {
        private static DBOperations _instance;
        public static DBOperations Instance => _instance ?? (_instance = new DBOperations());

        private DBOperations()
        {

        }


        /// <summary>
        /// Takes validated NumberWord object and appends it to the database
        /// </summary>
        /// <param name="nm"></param>
        public void AddNumberModel(NumberToWord nm)
        {
            using (var ent = SimplifyDBHelper.DBEntities)
            {
                ent.NumberToWords.Add(nm);
                ent.SaveChanges();
            }
        }

        /// <summary>
        /// Returns all numberword objects to validated user
        /// </summary>
        /// <param name="nm"></param>
        public IEnumerable<NumberToWord> GetAllNumberModels()
        {
            using (var ent = SimplifyDBHelper.DBEntities)
            {
                return ent.NumberToWords.ToList();
            }
        }
    }
}