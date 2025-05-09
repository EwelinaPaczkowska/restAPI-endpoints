using System.ComponentModel.DataAnnotations;
namespace webAPI.DTOs;

public class CClientDTO
{
    [Required]
    [MaxLength(120)]
    public string first_name { get; set; }
    [Required]
    [MaxLength(120)]
    public string last_name { get; set; }
    [Required]
    [EmailAddress]
    [MaxLength(120)]
    public string email { get; set; }
    [Required]
    [Phone]
    [MaxLength(120)]
    public string tel { get; set; }
    [Required]
    [Length(11,11)]
    [MaxLength(120)]
    public string pesel { get; set; }
}