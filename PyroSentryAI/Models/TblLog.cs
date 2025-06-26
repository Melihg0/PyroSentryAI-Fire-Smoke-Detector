using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PyroSentryAI.Models;

[Table("tbl_Logs")]
public partial class TblLog
{
    [Key]
    [Column("LogID")]
    public int LogId { get; set; }

    [Column("CameraID")]
    public int CameraId { get; set; }

    [StringLength(20)]
    public string EventType { get; set; } = null!;

    [Column(TypeName = "decimal(3, 2)")]
    public decimal ConfidenceScore { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Timestamp { get; set; }

    [StringLength(255)]
    public string? MediaPath { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [ForeignKey("CameraId")]
    [InverseProperty("TblLogs")]
    public virtual TblCamera Camera { get; set; } = null!;
}
