namespace Apresentacao.Profiles
{
    public class TokenConfiguration
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int ClockSkew { get; set; }
    }
}