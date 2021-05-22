using System.Text.RegularExpressions;

namespace Luger.Api.Common
{
    public static class Utils
    {
        public static string NormalizeBucketName(string bucket)
        {
            return Regex.Replace(bucket.Trim().ToLower(), "[^a-z0-9]", "");
        }

        public static string NormalizeLabelName(string label)
        {
            return Regex.Replace(label.Trim().ToLower(), "[^a-z0-9]", "");
        }
    }
}
