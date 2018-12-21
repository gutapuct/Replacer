using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Replacer.Models;
using static Replacer.Enums;

namespace Replacer.Hubs
{
    public class HubWebReporter : Hub
    {
        public void SendProgress(int max, int current, TypeProgressBar type, string connectionid)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<HubWebReporter>();
            context.Clients.Client(connectionid).sendProgress(max, current, type);
        }

        public void AddError(string message, string connectionid)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<HubWebReporter>();
            context.Clients.Client(connectionid).addError(message);
        }
    }
}