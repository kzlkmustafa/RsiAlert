namespace AlSatBot
{
    public class FormModel
    {
        public string Coins { get; set; }
        public int rsiBuyThreshold { get; set; }
        public int rsiSellThreshold { get; set; }
        public string hourPeriod { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
    }
}
