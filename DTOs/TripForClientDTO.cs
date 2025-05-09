namespace webAPI.DTOs;

public class TripForClientDTO
{
    public int IdT { get; set; }
    public string name { get; set; }
    public string desc { get; set; }
    public DateTime date_from { get; set; }
    public DateTime date_to { get; set; }
    public int max { get; set; }
    public int id_c { get; set; }
    public int registeret_at { get; set; }
    public int pay_date { get; set; }
}