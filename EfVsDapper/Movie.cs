using System.ComponentModel.DataAnnotations;

namespace EfVsDapper;

public class Movie
{
    [Key]
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int YearOfRelease { get; set; }
}