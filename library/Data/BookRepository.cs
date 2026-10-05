using library.Models;
using Npgsql;

namespace library.Data;

// Every SQL statement about books lives in this one class.
public class BookRepository
{
    private readonly NpgsqlDataSource _dataSource;

    // ASP.NET Core hands in the data source that Program.cs registered.
    public BookRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Copies the current row of the reader into a Book.
    // The positions 0 to 3 match the column order in both SELECTs above.
    private static Book ReadBook(NpgsqlDataReader reader)
    {
        return new Book
        {
            BookId = reader.GetInt64(0),
            Title = reader.GetString(1),
            Category = reader.IsDBNull(2) ? null : reader.GetString(2),
            Price = reader.IsDBNull(3) ? null : reader.GetDecimal(3)
        };
    }
}