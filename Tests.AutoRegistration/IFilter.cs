using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.AutoRegistration
{
    public interface IFilter<T>
    {
        T Execute(T input);
    }
}
