using library.Models;
using Npgsql;
namespace library.Data;

// Every SQL statement about books lives in this one class.
public class MemberRepository
{
    private readonly NpgsqlDataSource _dataSource;

    // ASP.NET Core hands in the data source that Program.cs registered.
    public MemberRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    // Copies the current row of the reader into a .
    // The positions 0 to 3 match the column order in both SELECTs above.
    private static Member ReadMember(NpgsqlDataReader reader)
    {
        return new Member
        {
            MemberId = reader.GetInt64(0),
            FullName = reader.GetString(1),
            Email = reader.IsDBNull(2) ? null : reader.GetString(2),   // NULL becomes null
            MemberType = reader.IsDBNull(3) ? null : reader.GetString(3),
            JoinDate = reader.GetDateTime(4)
        };
    }
    // READ: every member, sorted by full name.
    public async Task<List<Member>> GetAllAsync()
    {
        const string sql = "SELECT member_id, full_name, email, member_type, join_date FROM lending.member ORDER BY full_name;";

        var members = new List<Member>();

        // The command borrows a connection; 'await using' gives it back when the method ends.
        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())        // once for each row
        {
            members.Add(ReadMember(reader));
        }

        return members;
    }
    // CREATE: insert a new book. The database chooses its book_id.
    public async Task AddAsync(Member member)
    {
        const string sql = "INSERT INTO lending.member (full_name, email, member_type, join_date) " +
                           "VALUES (@full_name, @email, @member_type, @join_date);";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("full_name", member.FullName);
        command.Parameters.AddWithValue("email", (object?)member.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("member_type", (object?)member.MemberType ?? DBNull.Value);
        command.Parameters.AddWithValue("join_date", member.JoinDate);

        await command.ExecuteNonQueryAsync();           // runs SQL that returns no rows
    }


}
