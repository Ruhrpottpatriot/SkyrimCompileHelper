using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimCompileHelper.Common
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();

        T GetByIdentfier();
    }
}
