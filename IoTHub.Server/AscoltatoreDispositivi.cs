using IoTHub.TelemetryData;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTHub.Server
{
    /// <summary>
    /// Ascolta i messaggi dai dispositivi
    /// </summary>
    public class AscoltatoreDispositivi
    {
        // Endopoint, di default, sul quale il server riceve i messaggi
        private const string EndPointServer = "messages/events";
        // Endopoint, di default, sul quale i dispositivi mandano al server l'ACK. 
        //private const string Feedback = "messages/servicebound/feedback";

        public EventHubClient HubClient { get; set; }
        private string _connectionString;
        private MittenteServer _serverSender;

        public AscoltatoreDispositivi(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;

            HubClient = EventHubClient.CreateFromConnectionString(connectionString, EndPointServer);
            _serverSender = new MittenteServer(_connectionString);
        }

        /// <summary>
        /// Si mette in ascolto sugli ack relativi all'invio di un messaggio.
        /// </summary>
        /// <returns></returns>
        public async Task RicezioneMessaggiFeedback()
        {
            await _serverSender.AscoltaFeedbackAsync();
        }

        /// <summary>
        /// Ascolta i messaggi dei dispositivi
        /// </summary>
        /// <param name="partitionId"></param>
        /// <returns></returns>
        /// <remarks>
        /// Ogni volta che arriva un payload di dati:
        /// 1) deserializzo l'oggetto
        /// 2) invoco un metodo registrato sul device (un particolare device Id)
        /// 3) invio il messaggio a un device (un particolare device Id). Il messaggio inviato ha richiesto esplicitamente la risposta dal device tramite ACK.
        /// </remarks>
        public async Task RicezioneMessaggiDaDispositivi(string partitionId)
        {
            var eventHubReceiver = HubClient.GetDefaultConsumerGroup().CreateReceiver(partitionId, DateTime.UtcNow);
            while (true)
            {
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Telemetria nuovoDatoRicevuto = JsonConvert.DeserializeObject<Telemetria>(data);
                string dispositivoDatiSistema = eventData.SystemProperties["iothub-connection-device-id"].ToString();

                Console.WriteLine($"Dato scodato dal server: {nuovoDatoRicevuto}");

                // Faccio scattare il metodo registrato dal device
                await _serverSender.InvocaMetodoSulDevice(nuovoDatoRicevuto.Dispositivo);
                string messageToSend = JsonConvert.SerializeObject(new Telemetria { Dispositivo = "Server", VelocitaVento = 0, Latitudine = 0, Longitudine = 0 });
                await _serverSender.InviaAsync(nuovoDatoRicevuto.Dispositivo, messageToSend);
            }
        }

    }
}
