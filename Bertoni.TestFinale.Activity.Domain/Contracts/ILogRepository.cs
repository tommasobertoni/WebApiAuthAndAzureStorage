using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bertoni.TestFinale.Activity.Domain.Contracts
{
    public interface ILogRepository
    {
        void LogToRepository(string log);
    }
}
