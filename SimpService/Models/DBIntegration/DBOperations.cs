using System.Collections.Generic;
using System.Linq;

namespace SimpService.Models.DBIntegration
{
    public class DBOperations
    {
        private static DBOperations _instance;
        public static DBOperations Instance => _instance ?? (_instance = new DBOperations());

        private DBOperations()
        {

        }


        public void AddNumberModel(NumberWord nm)
        {
            using (var ent = SimplifyDBHelper.DBEntities)
            {
                ent.NumberWords.Add(nm);
                ent.SaveChanges();
            }
        }

        public IEnumerable<NumberWord> GetAllNumberModels()
        {
            using (var ent = SimplifyDBHelper.DBEntities)
            {
                return ent.NumberWords.ToList();
            }
        }
    }
}