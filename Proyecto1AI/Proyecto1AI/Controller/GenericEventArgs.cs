using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1AI.Controller
{
    public class GenericEventArgs<T> : EventArgs
    {
        public T EventData { get; private set; }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Constructor
        public GenericEventArgs(T eventData)
        {
            this.EventData = eventData;
        }

    }
}
