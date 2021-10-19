namespace Luger.Endpoints.Models
{
    public class Cursor
    {
        public string? Shard { get; set; }
        public long? Offset { get; set; }
    }
}
