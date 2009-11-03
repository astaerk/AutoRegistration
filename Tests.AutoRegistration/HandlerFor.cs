using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.AutoRegistration
{
    public interface IHandlerFor<TEvent>
    {
        void Handle(TEvent e);
    }
}
