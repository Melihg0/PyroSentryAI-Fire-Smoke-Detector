using PyroSentryAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSentryAI.Messages
{
    /// <summary>
    /// Bir kameranın eklendiğini bildiren mesaj,Record.
    /// </summary>       
    public record CameraAddedMessage(TblCamera NewCamera);       
    
}
