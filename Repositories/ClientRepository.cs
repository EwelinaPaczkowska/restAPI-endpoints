﻿using System.Data.SqlClient;
 using webAPI.DTOs;
 using webAPI.Exceptions;
 using webAPI.Models;
 using webAPI.Models;

public class ClientRepository: IClientRepository
{
    private readonly string _connectionString;
    
    public ClientRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IEnumerable<TripForClientDTO>> GetTripsForClientAsync(int id, CancellationToken cancellationToken)
    {
        var trips = new List<TripForClientDTO>();

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
                        [dbo].[Client_Trip].[IdClient],
                        [dbo].[Client_Trip].[RegisteredAt],
                        [dbo].[Client_Trip].[PaymentDate]
                        FROM [dbo].[Trip] 
                        Inner Join [dbo].[Client_Trip] On [dbo].[Client_Trip].[IdTrip] = [dbo].[Trip].[IdTrip]
                        WHERE [dbo].[Client_Trip].[IdClient] = @id";

            await using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var trip = new TripForClientDTO()
                        {
                            IdT = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                            name = reader.GetString(reader.GetOrdinal("Name")),
                            desc = reader.GetString(reader.GetOrdinal("Description")),
                            date_from = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            date_to = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            max = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                            id_c = reader.GetInt32(reader.GetOrdinal("IdClient")),
                            registeret_at = reader.GetInt32(reader.GetOrdinal("RegisteredAt")),
                            pay_date = reader.GetInt32(reader.GetOrdinal("PaymentDate"))
                        };
                        
                        trips.Add(trip);
                        
                    }
                    
                }
                
                
            }
            
        }
        
        if(trips.Count<1)
            throw new NotFoundException("client with id: " + id + " is not assigned to any trips");

        return trips;
    }

    public async Task<int> CreateNewClientAsync(Client client, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = @"
                        INSERT INTO [dbo].[Client]
                        ([FirstName], [LastName], [Email], [Telephone], [Pesel]) 
                        OUTPUT INSERTED.IdClient
                        VALUES
                            (@FirstName, @LastName, @Email, @Telephone, @Pesel)";
            await using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FirstName", client.first_name);
                command.Parameters.AddWithValue("@LastName", client.last_name);
                command.Parameters.AddWithValue("@Email", client.email);
                command.Parameters.AddWithValue("@Telephone", client.phone);
                command.Parameters.AddWithValue("@Pesel", client.pesel);
                
                var insertedid = await command.ExecuteScalarAsync(cancellationToken);
                
                return Convert.ToInt32(insertedid);
            }
        }
    }
    
    
    public async Task<bool> DoesPeselExistAsync(string pesel, CancellationToken cancellationToken)
    {
        
        await using (var connection = new SqlConnection(_connectionString)){
            
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT COUNT(1) FROM [dbo].[Client] WHERE Pesel = @pesel";
            
            await using (var command = new SqlCommand(query, connection)){
                command.Parameters.AddWithValue("@pesel", pesel);
                
                var result = await command.ExecuteScalarAsync(cancellationToken);
                
                return Convert.ToInt32(result) > 0;
            }
        }
        
    }
    
    public async Task<bool> DoesClientExistAsync(int id, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString)){
            
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT COUNT(1) FROM [dbo].[Client] WHERE IdClient = @id";
            
            await using (var command = new SqlCommand(query, connection)){
                command.Parameters.AddWithValue("@id", id);
                
                var result = await command.ExecuteScalarAsync(cancellationToken);
                
                return Convert.ToInt32(result) > 0;
            }
        }
    }
    
    public async Task<bool> DoesTripExistAsync(int id, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString)){
            
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT COUNT(1) FROM [dbo].[Trip] WHERE IdTrip = @id";
            
            await using (var command = new SqlCommand(query, connection)){
                command.Parameters.AddWithValue("@id", id);
                
                var result = await command.ExecuteScalarAsync(cancellationToken);
                
                return Convert.ToInt32(result) > 0;
            }
        }
    }
    
    public async Task<bool> DoesClientTripAssignmentExist(int ClientId, int TripId, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString)){
            
            await connection.OpenAsync(cancellationToken);

            var query = @"SELECT COUNT(1) FROM [dbo].[Client_Trip] WHERE IdTrip = @TripId AND IdClient = @ClientId";
            
            await using (var command = new SqlCommand(query, connection)){
                command.Parameters.AddWithValue("@TripId", TripId);
                command.Parameters.AddWithValue("@ClientId", ClientId);
                
                var result = await command.ExecuteScalarAsync(cancellationToken);
                
                return Convert.ToInt32(result) > 0;
            }
        }
    }
    
    public async Task<int> HowManyPeopleAreAssignedToTripAsync(int TripId, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString)){
            
            await connection.OpenAsync(cancellationToken);

            var query = @"
                        SELECT IdTrip, COUNT(IdClient) AS number
                        FROM [dbo].[Client_Trip] 
                        GROUP BY IdTrip
                        HAVING IdTrip = @TripId";
            
            await using (var command = new SqlCommand(query, connection)){
                command.Parameters.AddWithValue("@TripId", TripId);
                
                await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    var num = 0;

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        num = reader.GetInt32(reader.GetOrdinal("number"));
                        
                    }
                    return num;
                }
            }
        }
    }
    
    
    public async Task<int> MaxPeopleOnTrip(int TripId, CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString)){
            
            await connection.OpenAsync(cancellationToken);

            var query = @"
                        SELECT MaxPeople
                        FROM [dbo].[Trip] 
                        Where IdTrip = @TripId";
            
            await using (var command = new SqlCommand(query, connection)){
                command.Parameters.AddWithValue("@TripId", TripId);
                
                var result = await command.ExecuteScalarAsync(cancellationToken);
                
                return Convert.ToInt32(result);
            }
        }
    }

    public async Task AssignClientToTripAsync(int id, int tripId,int registrationDate, int? paymentDate , CancellationToken cancellationToken)
    {
        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = @"
                        INSERT INTO [dbo].[Client_Trip]
                        ([IdClient], [IdTrip], [RegisteredAt], [PaymentDate]) 
                        VALUES
                            (@IdClient, @IdTrip, @RegisteredAt, @PaymentDate)";
            await using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IdClient", id);
                command.Parameters.AddWithValue("@IdTrip", tripId);
                command.Parameters.AddWithValue("@RegisteredAt", registrationDate);
                command.Parameters.AddWithValue("@PaymentDate", paymentDate.HasValue ? paymentDate.Value : (object)DBNull.Value);

                

                var num = await command.ExecuteNonQueryAsync(cancellationToken);

                if (!(num > 0))
                {
                    throw new Exception("0 rows affected, data base didnt add new assignment");
                }
            }
        }
    }

    public async Task DeleteClientToTripAssignmentAsync(int id, int tripId, CancellationToken cancellationToken)
    {
        
        await using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);

            var query = @"
                        DELETE FROM [dbo].[Client_Trip]
                        WHERE 
                            [dbo].[Client_Trip].[iDClient] = @IdClient AND
                            [dbo].[Client_Trip].[IdTrip] = @IdTrip";
            await using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IdClient", id);
                command.Parameters.AddWithValue("@IdTrip", tripId);
                
                var num = await command.ExecuteNonQueryAsync(cancellationToken);

                if (!(num > 0))
                {
                    throw new Exception("0 rows affected, data base didnt remove any records");
                }
            }
        }
    }
}