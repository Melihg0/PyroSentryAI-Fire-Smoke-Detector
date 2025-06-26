using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PyroSentryAI.Models;

[Table("tbl_Users")]
[Index("Username", Name = "UQ__tbl_User__536C85E409C1B34B", IsUnique = true)]
public partial class TblUser
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(30)]
    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    [StringLength(100)]
    public string? FullName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("RoleID")]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("TblUsers")]
    public virtual TblRole Role { get; set; } = null!;
}
