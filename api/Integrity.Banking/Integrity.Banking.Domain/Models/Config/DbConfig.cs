namespace Integrity.Banking.Domain.Models.Config
{
    public class DbConfig
    {
        public string ConnectionString { get; set; } = "server=localhost;user=root;password=password;database=banking";
        public int MaxRetry { get; set; } = 3;
        public int RetryDelayInMs { get; set; } = 1000;
    }
}
