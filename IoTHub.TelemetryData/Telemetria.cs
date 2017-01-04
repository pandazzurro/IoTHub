using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHub.TelemetryData
{
    public class Telemetria
    {
        public double VelocitaVento { get; set; }
        public string Dispositivo { get; set; }
        public double Latitudine { get; set; }
        public double Longitudine { get; set; }

        public override string ToString()
        {
            return $"{Dispositivo} - [{VelocitaVento}; {Latitudine}-{Longitudine}]";
        }
    }
}
