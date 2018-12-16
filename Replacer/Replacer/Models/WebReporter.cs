using Replacer.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replacer.Models
{
    public class WebReporter
    {
        private HubWebReporter hub = new HubWebReporter();

        public void SendProgress(int max, int current)
        {
            hub.SendProgress(max, current);
        }

        public void AddError(string message)
        {
            hub.AddError(message);
        }
    }
}
