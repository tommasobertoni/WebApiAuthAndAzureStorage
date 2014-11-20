using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bertoni.TestFinale.Activity.Domain
{
    public class ActivityCategory
    {
        private static Dictionary<int, string> categories = new Dictionary<int, string>
        {
            {0, "Generic activity"},
            {1, "Work activity"},
            {2, "Activity to do at home"},
            {3, "Hobby activity"}
        };

        public static string GetCategoryDesctiption(int categoryId)
        {
            return categories[categoryId];
        }
    }
}
