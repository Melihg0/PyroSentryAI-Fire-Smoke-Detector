using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PyroSentryAI.Models;

[Table("tbl_Role")]
public partial class TblRole
{
    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(20)]
    public string RoleName { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<TblUser> TblUsers { get; set; } = new List<TblUser>();
}
