using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bertoni.TestFinale.Domain.Contracts
{
    public interface IActivityRepository
    {
        IEnumerable<ActivityDTO> GetAll(string username);

        ActivityDTO GetById(string username, string id);

        void Insert(string username, ActivityDTO activity);

        void Update(string username, ActivityDTO activity);

        void DeleteById(string username, string id);
    }
}
