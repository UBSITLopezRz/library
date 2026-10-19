namespace library.Models;

public class Book
{
    public long BookId { get; set; }

    public string Title { get; set; } = "";

    public string? Category { get; set; }

    public decimal? Price { get; set; }
}