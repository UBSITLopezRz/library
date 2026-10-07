namespace library.Models;

// One Book object holds one row of lending.book.
public class Member
{
    public long MemberId { get; set; }          // book_id   BIGINT, the primary key
    public string FullName { get; set; } = "";   // title     VARCHAR(150) NOT NULL
    public string? email { get; set; }     // category  VARCHAR(40), may be NULL
    public decimal? MemberType { get; set; }       // price     NUMERIC(7,2), may be NULL
    public DateTime join_date { get; set; } 
}