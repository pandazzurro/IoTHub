using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace IoTHub.Devices
{
    class Program
    {
        private static string connectionString;
        private static string HubUri;
        private static BlockingCollection<int> DeviceNumber = new BlockingCollection<int>();
        static void Main(string[] args)
        {
            connectionString = ConfigurationManager.AppSettings["IoTHub"];
            HubUri = ConfigurationManager.AppSettings["IoTHubUri"];
            List<Task> tasks = new List<Task>();

            for(int i = 0; i < 20; i++)
            {
                var t = Task.Run(async () =>
                {
                    GestisciDispositivo gestisciDispositivo = new GestisciDispositivo($"dispositivo{GetDeviceNumber()}", connectionString);
                    Device dispositivo = await gestisciDispositivo.RegistraDispositivoAsync();

                    DeviceClient client = DeviceClient.Create(HubUri,
                        new DeviceAuthenticationWithRegistrySymmetricKey(dispositivo.Id, dispositivo.Authentication.SymmetricKey.PrimaryKey),
                        Microsoft.Azure.Devices.Client.TransportType.Mqtt_WebSocket_Only);
                    MittenteDispositivo mittente = new MittenteDispositivo(dispositivo, client);

                    await mittente.RegistraMetodoSulDispositivoAsync();
                    mittente.InvioDatiAsync();
                    mittente.RiceviDatiAsync();
                    Console.Read();
                });
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());
        }
        
        /// <summary>
        /// Assegna un numero incrementale del nome del dispositivo
        /// </summary>
        /// <returns></returns>
        public static int GetDeviceNumber()
        {
            DeviceNumber.TryAdd(DeviceNumber.Count());
            return DeviceNumber.Count();
        }      
    }
}
