namespace SuSuerteV2.Domain.ApiService.Models
{
    public static class StringExtensions
    {
        public static string ToCapitalized(this string str)
        {
            switch (str)
            {
                case null: throw new ArgumentNullException();
                case "": return str;
                default:
                    {
                        string res = string.Empty;
                        var words = str.ToLower().Split(' ');
                        foreach (string word in words)
                        {
                            res += word[0].ToString().ToUpper() + word.Substring(1) + " ";

                        }

                        return res.Trim();
                    }
            }
        }
    }
}