
using System;
using System.Data;
using Microsoft.Data.SqlClient;

// SQL Server LocalDB instance
string server = @"(localdb)\MSSQLLocalDB";

// Connect to master database first
string masterCS =
    $"Server={server};Database=master;" +
    "Integrated Security=True;" +
    "TrustServerCertificate=True;";

string databaseCS =
    $"Server={server};Database=HandbookDemoDB;" +
    "Integrated Security=True;" +
    "TrustServerCertificate=True;";

try
{
    // Step 1: Create database if it does not exist
    using (var master = new SqlConnection(masterCS))
    {
        master.Open();

        string createDatabase = """
            IF DB_ID('HandbookDemoDB') IS NULL
                EXEC('CREATE DATABASE HandbookDemoDB');
            """;

        using var cmd = new SqlCommand(createDatabase, master);
        cmd.ExecuteNonQuery();

        Console.WriteLine("Database ready.");
    }

    // Step 2: Connect to HandbookDemoDB
    using var connection = new SqlConnection(databaseCS);
    connection.Open();

    Console.WriteLine("Connected to HandbookDemoDB.");

    // Step 3: Create Customers table if it does not exist
    string createTable = """
        IF OBJECT_ID('dbo.Customers', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Customers
            (
                CustomerId INT IDENTITY(1,1) PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL
            );
        END
        """;

    using (var cmd = new SqlCommand(createTable, connection))
    {
        cmd.ExecuteNonQuery();
    }

    Console.WriteLine("Customers table ready.");

    // Step 4: Insert customer record
    using (var cmd = new SqlCommand(
        "INSERT INTO dbo.Customers(Name) VALUES (@name)",
        connection))
    {
        cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100)
            .Value = "Amina";

        int affected = cmd.ExecuteNonQuery();

        Console.WriteLine($"Rows inserted: {affected}");
    }

    // Step 5: Display customer records
    using (var cmd = new SqlCommand(
        "SELECT CustomerId, Name FROM dbo.Customers",
        connection))
    using (var reader = cmd.ExecuteReader())
    {
        Console.WriteLine("\nCustomer Records:");

        while (reader.Read())
        {
            Console.WriteLine(
                $"{reader.GetInt32(0)}: {reader.GetString(1)}");
        }
    }
}
catch (SqlException ex)
{
    Console.WriteLine($"SQL Error {ex.Number}: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}


