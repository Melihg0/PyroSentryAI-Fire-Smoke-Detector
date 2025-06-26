using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSentryAI.Messages
{
    /// <summary>
    /// Bir kameranın(CameraViewModel) durumu değiştiğinde Id'sini CameraName'ini ve etiketini bildiren mesaj,Record.
    /// </summary>       
    public record AlarmStateChangedMessage(int CameraId, string CameraName, List<string> DetectedLabels);
}
    

