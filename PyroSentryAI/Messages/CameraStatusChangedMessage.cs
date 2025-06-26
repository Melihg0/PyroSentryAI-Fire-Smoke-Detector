using PyroSentryAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSentryAI.Messages
{
    /// <summary>
    /// Bir kameranın aktif pasif durumunun degistigini bildiren mesaj,Record.
    /// </summary>   
    public record CameraStatusChangedMessage(TblCamera UpdatedCamera);
   
}
