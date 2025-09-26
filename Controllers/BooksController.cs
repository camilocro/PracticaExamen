using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace TecWeb_Practica_Examen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private static readonly List<Book> _books = new()
        {
            new Book { Id = Guid.NewGuid(), Title = "100 anios de soledad", Author = "Gabriel Garcia", Genre = "Dramatico", PublicationYear = 1965 },
            new Book { Id = Guid.NewGuid(), Title = "Harry Potter", Author = "Emma Stone", Genre = "Lirico", PublicationYear = 2000 }
        };

        private static(int page, int limit) NormalizePage(int? page, int? limit)
        {
            var p = page.GetValueOrDefault(1); if (p < 1) p = 1;
            var l = limit.GetValueOrDefault(10); if(l < 1) l = 1; if(l>100) l= 100;
            return (p, l);
        }
        private static IEnumerable<T> OrderByProp<T>(IEnumerable<T> src, string? sort, string? order)
        {
            if (string.IsNullOrEmpty(sort)) return src;
            var prop = typeof(T).GetProperty(sort, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop is null) return src;

            return string.Equals(order,"desc", StringComparison.OrdinalIgnoreCase)
                ? src.OrderByDescending(x=> prop.GetValue(x)):
                src.OrderBy(x=> prop.GetValue(x));
        }
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] int? page, [FromQuery] int? limit,
            [FromQuery] string? sort, [FromQuery] string? order,
            [FromQuery] string? q, [FromQuery] string? genre)
        {
            var (p, l) = NormalizePage(page, limit);
            IEnumerable<Book> query = _books;
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a => a.Title.Contains(q, StringComparison.OrdinalIgnoreCase) || a.Genre.Contains(q, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(a => a.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
            }
            query= OrderByProp(query, sort, order);
            var total = query.Count();
            var data= query.Skip((p-1)*l).Take(l).ToList();
            return Ok(new
            {
                data,
                meta = new { page = p, limit = l, total }
            });
        }
        [HttpGet("{id:guid}")]
        public ActionResult<Book> GetOne(Guid id)
        {
            var book = _books.FirstOrDefault(a => a.Id == id);
            return book is null ? NotFound() : Ok(book);
        }
        [HttpPost]
        public ActionResult<Book> Create([FromBody] CreateBookDto dto)
        {
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = dto.Title.Trim(),
                Author = dto.Author.Trim(),
                Genre = dto.Genre.Trim(),
                PublicationYear = dto.PublicationYear
            };
            _books.Add(book);
            return CreatedAtAction(nameof(GetOne), new {id=book.Id},book);
        }
        [HttpPut("{id:guid}")]
        public ActionResult<Book> Update(Guid id, [FromBody] UpdateBookDto dto)
        {
            var index = _books.FindIndex(a => a.Id == id);
            if (index == -1) return NotFound();
            var updated = new Book
            {
                Id = id,
                Title = dto.Title.Trim(),
                Author = dto.Author.Trim(),
                Genre = dto.Genre.Trim(),
                PublicationYear = dto.PublicationYear
            };
            _books[index] = updated;
            return Ok(updated);
        }
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id) {
            var removed=_books.RemoveAll(a => a.Id == id);
            return removed ==0? NotFound() : NoContent();
        }

    }

}