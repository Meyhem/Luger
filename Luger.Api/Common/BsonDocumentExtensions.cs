using MongoDB.Bson;

namespace Luger.Api.Common
{
    public static class BsonDocumentExtensions
    {
        public static string GetStringOrEmpty(this BsonDocument d, string field)
        {
            if (d.TryGetValue(field, out var value))
            {
                return value.AsString;
            }
            return string.Empty;
        }
    }
}
