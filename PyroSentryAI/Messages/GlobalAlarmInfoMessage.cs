using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyroSentryAI.Messages
{
    /// <summary>
    ///  Genel alarm durumunu ve detaylarını bildiren mesaj,Record.
    /// </summary>
    public record GlobalAlarmInfoMessage(bool IsAlarmActive, List<string> AlarmDetails);
}
