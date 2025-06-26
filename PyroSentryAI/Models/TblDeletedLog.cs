using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PyroSentryAI.Models;

[Keyless]
[Table("tbl_DeletedLogs")]
public partial class TblDeletedLog
{
    [Column("LogID")]
    public int LogId { get; set; }

    [Column("CameraID")]
    public int CameraId { get; set; }

    [StringLength(20)]
    public string? EventType { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal? ConfidenceScore { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Timestamp { get; set; }

    [StringLength(255)]
    public string? MediaPath { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DeletedAt { get; set; }

    [Column("DeletedByUserID")]
    public int? DeletedByUserId { get; set; }
}
