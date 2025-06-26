using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PyroSentryAI.Models;

[Table("tbl_Cameras")]
[Index("CameraName", Name = "UQ__tbl_Came__4F698F2B273D1D67", IsUnique = true)]
public partial class TblCamera
{
    [Key]
    [Column("CameraID")]
    public int CameraId { get; set; }

    [StringLength(50)]
    public string CameraName { get; set; } = null!;

    [Column("RTSPUrl")]
    [StringLength(255)]
    public string Rtspurl { get; set; } = null!;

    public bool? IsActive { get; set; }

    [InverseProperty("Camera")]
    public virtual ICollection<TblLog> TblLogs { get; set; } = new List<TblLog>();
}
