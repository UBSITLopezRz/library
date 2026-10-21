using library.Models;
using Npgsql;

namespace library.Data;

// Every SQL statement about notes lives in this one class.
public class NoteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NoteRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Copies the current row into a Note.
    private static Note ReadNote(NpgsqlDataReader reader)
    {
        return new Note
        {
            NoteId = reader.GetInt64(0),
            Title = reader.GetString(1),
            Body = reader.IsDBNull(2) ? null : reader.GetString(2),
            CreatedOn = reader.GetDateTime(3)
        };
    }

    // READ: every note, sorted by date.
    public async Task<List<Note>> GetAllAsync()
    {
        const string sql =
            "SELECT note_id, title, body, created_on " +
            "FROM lending.note ORDER BY created_on, note_id;";

        var notes = new List<Note>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            notes.Add(ReadNote(reader));
        }

        return notes;
    }
    // CREATE: insert a new note. The database chooses its note_id.
    public async Task AddAsync(Note note)
    {
        const string sql = "INSERT INTO lending.note (title, body, created_on) " +
                           "VALUES (@title, @body, @createdOn);";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue("body", (object?)note.Body ?? DBNull.Value);
        command.Parameters.AddWithValue("createdOn", note.CreatedOn);

        await command.ExecuteNonQueryAsync();           // runs SQL that returns no rows
    }
    // READ: one book, or null when no book has that id.
    public async Task<Note?> GetByIdAsync(long id)
    {
        const string sql = "SELECT note_id, title, body, created_on FROM lending.note WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);      // @id in the SQL receives this value

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? ReadNote(reader) : null;
    }
    // UPDATE: change every column of one book.
    public async Task UpdateAsync(Note note)
    {
        const string sql = "UPDATE lending.note " +
                           "SET title = @title, body = @body " +
                           "WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("title", note.Title);
        command.Parameters.AddWithValue("body", (object?)note.Body ?? DBNull.Value);
        command.Parameters.AddWithValue("id", note.NoteId);

        await command.ExecuteNonQueryAsync();
    }
    // DELETE: remove one note.
    public async Task DeleteAsync(long id)
    {
        const string sql = "DELETE FROM lending.note WHERE note_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }





}