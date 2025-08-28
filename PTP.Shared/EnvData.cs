namespace PTP.Shared
{
    public class EnvData
    {
        public static string EnvCurrentDirectory => Directory.GetCurrentDirectory();
        public static string EnvDirectoryRoot => Directory.GetDirectoryRoot(EnvCurrentDirectory);
        public static string EnvDirectory => Path.Combine(Directory.GetParent(EnvCurrentDirectory)?.Parent?.Parent?.Parent?.FullName!);
    }
}
