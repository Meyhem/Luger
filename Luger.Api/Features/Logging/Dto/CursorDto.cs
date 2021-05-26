namespace Luger.Api.Features.Logging.Dto
{
    public class CursorDto
    {
        public string? Chunk { get; set; }

        public long Offset { get; set; }

        public int Limit { get; set; }
    }
}
