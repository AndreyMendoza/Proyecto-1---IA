using System.Threading;

namespace Proyecto1AI.Model
{
    class MainController
    {
        private Board Board;
        public string NextAction { get; set; } = "";
        public Semaphore mutex { get; set; } = new Semaphore(0, 1, "NextAction");
        
        public MainController()
        {
            
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Reads the NextAction variable and execute it
        public void ExecuteNextAction()
        {
            // Block the actions in order 
            
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------
        
        

    }
}
