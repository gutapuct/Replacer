using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Replacer.Models;

namespace Replacer.Hubs
{
    public class HubWebReporter : Hub
    {
        public void SendProgress(int max, int current)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<HubWebReporter>();
            context.Clients.All.sendProgress(max, current); 
        }
    }
}