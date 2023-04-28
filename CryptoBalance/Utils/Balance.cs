namespace CryptoBalance.Utils
{
    public static class Balance
    {
        public static bool IsSynchrozed { get; set; } = false;
        public static Dictionary<(int Id, string Address), decimal?> Data { get; set; } = new Dictionary<(int Id, string Address), decimal?>();
    }
}
