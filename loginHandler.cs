using Grimoire.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamplePacketPlugin
{
    public class loginHandler : IXtMessageHandler
    {
        public string[] HandledCommands { get; } = { "firstJoin" };

        public bool loggedInState = false;

        public void Handle(XtMessage message)
        {
            loggedInState = true;
        }

        // get current first join state and set it to false
        public async Task<bool> getSetStateAsync()
        {
            bool currState = loggedInState;
            this.loggedInState = false;
            return currState;
        }

        // get first join state
        public async Task<bool> getStateAsync()
        {
            return this.loggedInState;
        }
    }
}
