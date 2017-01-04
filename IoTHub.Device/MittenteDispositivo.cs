using IoTHub.TelemetryData;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTHub.Devices
{
    public class MittenteDispositivo
    {
        private Device _dispositivo;
        private DeviceClient _client;
        public MittenteDispositivo(Device dispositivo, DeviceClient client)
        {
            if (dispositivo == null)
                throw new ArgumentNullException(nameof(dispositivo));

            if (client == null)
                throw new ArgumentNullException(nameof(client));
            _dispositivo = dispositivo;
            _client = client;            
        }

        /// <summary>
        /// Invio dati al Server
        /// </summary>
        /// <returns></returns>
        public async Task InvioDatiAsync()
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();
            while (true)
            {
                Telemetria datiTelemetria = new Telemetria
                {
                    Dispositivo = _dispositivo.Id,
                    VelocitaVento = avgWindSpeed + rand.NextDouble() * 4 - 2,
                    Latitudine = 45,
                    Longitudine = 45
                };
                var messageString = JsonConvert.SerializeObject(datiTelemetria);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));
                
                
                await _client.SendEventAsync(message);
                Console.WriteLine($"Dato invito dal dispostivo: {datiTelemetria}");
                Task.Delay(5000).Wait();
            }
        }

        /// <summary>
        /// Metodo che ascolta i messaggi del server inviati a un particolare device
        /// </summary>
        /// <returns></returns>
        public async Task RiceviDatiAsync()
        {
            while (true)
            {
                Microsoft.Azure.Devices.Client.Message messaggioRicevuto = await _client.ReceiveAsync();
                // Se non arrivano messaggi nell'ultimo minuto (default value) il messaggio risulta NULL.
                if (messaggioRicevuto == null) continue;

                // Le operazioni sul messaggio dovranno avere la logica di IDEMPOTENZA.
                // L'esecuzione dell'azione potrebbe fallire e il messaggio potrebbe essere ripristinato
                string messaggio = Encoding.ASCII.GetString(messaggioRicevuto.GetBytes());
                Telemetria datoRicevuto = JsonConvert.DeserializeObject<Telemetria>(messaggio);
                Console.WriteLine($"Dato ricevuto dal dispostivo: {_dispositivo.Id} - {datoRicevuto}");

                await _client.CompleteAsync(messaggioRicevuto); // sblocco il messaggio e notifico che è stato ricevuto correttamente
            }
        
        }

        /// <summary>
        /// Registrazione del metodo richiamato dal server.
        /// </summary>
        /// <returns></returns>
        public async Task RegistraMetodoSulDispositivoAsync()
        {
            DateTimeOffset dataCorrente = DateTimeOffset.UtcNow;
            _client.SetMethodHandler("MetodoInvocatoDaServer", MetodoInvocatoDaServer, dataCorrente);
        }

        /// <summary>
        /// Logica del metodo richiamato dal server.
        /// </summary>
        /// <param name="methodRequest"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        public async Task<MethodResponse> MetodoInvocatoDaServer(MethodRequest methodRequest, object userContext)
        {
            string clientVerbose =  $"Metodo invocato dal client: Attivazione lavori in data {((DateTimeOffset)userContext).Date.ToShortDateString()}";
            MethodResponseData serverResponse = new MethodResponseData
            {
                DataResult = $"Metodo invocato dal server: Attivazione lavori in data {((DateTimeOffset)userContext).Date.ToShortDateString()}"
            };
            Console.WriteLine(clientVerbose);
            MethodResponse response = new MethodResponse(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(serverResponse)), 0);
            return response;
        }
    }
}
