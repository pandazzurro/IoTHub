# IoTHub
In questo progetto viene testato l'IoTHub mettendo in comunicazione alcuni dispositivi virtuali con un Back-end

### Device
  - Creazione di una identità per il dispositivo
  - Pubblicazione di un file blob sull'account di archiviazione
  - Utilizzo del protocollo MQTT
  - Invio dati dal dispositivo al back-end
  - Ricezione dei dati dal dispositivo al back-end
  
### Back-end
  - Invio dei dati dal Back-end al device
  - Ricezione dei dati dal Back-end
  - Ricezione dell'ACK di un messaggio inviato dal Back-end verso un dispositivo
 
### Installazione ed esecuzione
 * Creare una **`Sottoscrizione azure`** e **`IoTHub`**.
 * Scaricare il progetto e modificare le stringhe di connessione presenti nei file app.config.
 * Lanciare, con l'avvio multiplo, i progetti **IoTHub.Devices**, **IoTHub.Server**
