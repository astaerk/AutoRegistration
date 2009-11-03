using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.AutoRegistration
{
    public class DomainEventHandlerOne : IHandlerFor<DomainEvent>
    {
        #region IHandlerFor<DomainEvent> Members

        public void Handle(DomainEvent e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DomainEventHandlerTwo : IHandlerFor<DomainEvent>
    {
        #region IHandlerFor<DomainEvent> Members

        public void Handle(DomainEvent e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
