namespace Luger.Endpoints.Models
{
    public class RequestCreateToken
    {
        public string UserId { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
