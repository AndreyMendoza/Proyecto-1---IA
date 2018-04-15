using System;

namespace Proyecto1AI.Controller
{
    public class GenericEventArgs<T> : EventArgs
    {
        public T EventData { get; private set; }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Constructor
        public GenericEventArgs(T eventData)
        {
            EventData = eventData;
        }

    }
}
