using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.AutoRegistration
{
    public class Filter<T> : IFilter<T>
    {
        public T Execute(T input)
        {
            return input;
        }
    }
}
