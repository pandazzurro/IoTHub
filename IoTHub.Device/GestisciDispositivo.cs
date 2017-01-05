using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHub.Devices
{
    public class GestisciDispositivo
    {
        private string _nomeDispositivo;
        private Device _dispositivoCorrente;
        public Device DispositivoCorrente => _dispositivoCorrente == null ? new Device(_nomeDispositivo) : _dispositivoCorrente;
        private RegistryManager _registryManager;
        private string _connectionString;

        public GestisciDispositivo(string nomeDispositivo, string connectionString)
        {
            if (string.IsNullOrEmpty(nomeDispositivo))
                throw new ArgumentNullException(nameof(nomeDispositivo));
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            _nomeDispositivo = nomeDispositivo;
            _connectionString = connectionString;
            _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
        }

        /// <summary>
        /// Registra il dispositivo o ne recupera la sua identità
        /// </summary>
        /// <returns></returns>
        public async Task<Device> RegistraDispositivoAsync()
        {
                Device dispositivoRegistrato = await _registryManager.GetDeviceAsync(_nomeDispositivo);
                if (dispositivoRegistrato == null)
                    dispositivoRegistrato = await _registryManager.AddDeviceAsync(DispositivoCorrente);

                _dispositivoCorrente = dispositivoRegistrato;
                Console.WriteLine("Identità dispositivo: {0}", DispositivoCorrente.Authentication.SymmetricKey.PrimaryKey);
                return DispositivoCorrente;
        }

        /// <summary>
        /// Rimuove il dispositivo registrato
        /// </summary>
        /// <param name="nomeDispositivo"></param>
        /// <returns></returns>
        public async Task RimuoviDispositivoRegistrato()
        {
            Device dispositivoRegistrato = await _registryManager.GetDeviceAsync(_nomeDispositivo);
            if(dispositivoRegistrato != null)
                await _registryManager.RemoveDeviceAsync(_nomeDispositivo);
        }
               
    }
}
