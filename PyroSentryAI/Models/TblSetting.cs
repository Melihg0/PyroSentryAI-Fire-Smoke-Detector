using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PyroSentryAI.Models;


[Table("tbl_Settings")]
public partial class TblSetting
{
    [Key] 
    public int SettingID { get; set; }
    [Column(TypeName = "decimal(3, 2)")]
    public decimal ConfidenceThreshold { get; set; }

    [Column("AnalysisFPS")]
    public int AnalysisFps { get; set; }
}
