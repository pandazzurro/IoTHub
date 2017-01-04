using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTHub.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["IoTHub"];
            AscoltatoreDispositivi sender = new AscoltatoreDispositivi(connectionString);            
            var tasks = new List<Task>();
            foreach (string partition in sender.HubClient.GetRuntimeInformation().PartitionIds)
            {
                tasks.Add(sender.RicezioneMessaggiDaDispositivi(partition)); // mi metto in ascolto dei messaggi dei device
                tasks.Add(sender.RicezioneMessaggiFeedback()); // mi metto in ascolto degli ack da parte dei device
            }
            
            Task.WaitAll(tasks.ToArray());
        }
    }
}
