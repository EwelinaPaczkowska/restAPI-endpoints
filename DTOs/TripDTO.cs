using webAPI.Models;
namespace webAPI.DTOs;

public class TripDTO
{
    public int IdT { get; set; }
    public string name { get; set; }
    public string desc { get; set; }
    public DateTime date_from { get; set; }
    public DateTime date_to { get; set; }
    public int max { get; set; }
    public List<Country> Country { get; set; }
}