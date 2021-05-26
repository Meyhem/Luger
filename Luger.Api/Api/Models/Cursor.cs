using Luger.Api.Features.Logging.Dto;

namespace Luger.Api.Api.Models
{
    public class Cursor
    {
        public string? Chunk { get; set; } 

        public long Offset { get; set; }

        public int Limit { get; set; }

        public CursorDto ToDto()
        {
            return new()
            {
                Offset = Offset,
                Chunk = Chunk,
                Limit = Limit
            };
        }

        public static Cursor FromDto(CursorDto c)
        {
            return new()
            {
                Chunk = c.Chunk,
                Limit = c.Limit,
                Offset = c.Offset
            };
        }
    }
}
