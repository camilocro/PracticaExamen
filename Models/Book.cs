using System;
using System.ComponentModel.DataAnnotations;
public class Book
{
    public Guid Id { get; set; }
    [Required, StringLength(150)]
    public string Title { get; set; }

    [Required, StringLength(100)]
    public string Author { get; set; }
    [Required, StringLength(50)]
    public string Genre { get; set; }
    [Range(1800,2025)]
    public int PublicationYear { get; set; }
}
    
public record CreteBookDto
{
    [Required, StringLength(150)]
    public string Title { get; set; }

    [Required, StringLength(100)]
    public string Author { get; set; }
    [Required, StringLength(50)]
    public string Genre { get; set; }
    [Range(1800, 2025)]
    public int PublicationYear { get; set; }
}

public record UpdateBookDto
{
    [Required, StringLength(150)]
    public string Title { get; set; }

    [Required, StringLength(100)]
    public string Author { get; set; }
    [Required, StringLength(50)]
    public string Genre { get; set; }
    [Range(1800, 2025)]
    public int PublicationYear { get; set; }
}
