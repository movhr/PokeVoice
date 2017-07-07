using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public partial class Game
    {
        public class EventLink
        {
            private readonly Action _nextAction;

            public EventLink(Action nextAction)
            {
                _nextAction = nextAction;
            }

            public void Execute()
            {
                _nextAction();
            }
        }
    }
}
