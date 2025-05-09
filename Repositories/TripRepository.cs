 using System.Data.SqlClient;
 using webAPI.DTOs;
 using webAPI.Models;
 using webAPI.Repositories;

 namespace webAPI.Repositories;

public class TripRepository: ITripRepository
{
    private readonly string _connectionString;
    public TripRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<TripDTO>> GetTripsAsync(CancellationToken cancellationToken)
    {
        var trips = new List<TripDTO>();

        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            
            var query = @"SELECT 
                        [dbo].[Trip].[IdTrip], 
                        [dbo].[Trip].[Name], 
                        [dbo].[Trip].[Description],
                        [dbo].[Trip].[DateFrom],
                        [dbo].[Trip].[DateTo],
                        [dbo].[Trip].[MaxPeople],
                        [dbo].[Country].[Name] as [CountryName],
                        [dbo].[Country].[IdCountry]
                        FROM [dbo].[Trip] 
                        Inner Join [dbo].[Country_Trip] On [dbo].[Trip].[IdTrip] = [dbo].[Country_Trip].[IdTrip] 
                        Inner Join [dbo].[Country] On [dbo].[Country_Trip].[IdCountry] = [dbo].[Country].[IdCountry]";

            await using (var command = new SqlCommand(query, connection))
            {

                await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("IdTrip"));
                        if(trips.Any(t => t.IdT == id))
                        {
                            trips.Find(t => t.IdT == id).Country.Add(
                                    new Country
                                    {
                                        name = reader.GetString(reader.GetOrdinal("CountryName")),
                                        id_country = reader.GetInt32(reader.GetOrdinal("IdCountry"))
                                    }
                                    );
                        }
                        else
                        {
                            var trip = new TripDTO
                            {
                                IdT = id,
                                name = reader.GetString(reader.GetOrdinal("Name")),
                                desc = reader.GetString(reader.GetOrdinal("Description")),
                                date_from = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                                date_to = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                                max = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                            };

                            trip.Country = new List<Country>();

                            trip.Country.Add(
                                new Country
                                {
                                    name = reader.GetString(reader.GetOrdinal("CountryName")),
                                    id_country = reader.GetInt32(reader.GetOrdinal("IdCountry"))
                                });
                            trips.Add(trip);
                        }
                    }
                    
                }
                
                
            }
            
        }

        return trips;
    }
}