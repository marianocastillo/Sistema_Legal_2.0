using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Legal_2._0.Server.Models;

public class Credentials
{
    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string UserName { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Password { get; set; }
}