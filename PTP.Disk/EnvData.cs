namespace PTP.Disk
{
    internal class EnvData
    {
        public static string EnvCurrentDirectory => Directory.GetCurrentDirectory();
        public static string EnvDirectoryRoot => Directory.GetDirectoryRoot(EnvCurrentDirectory);
        public static string EnvDirectory => EnvDirectoryRoot + "Databases\\PTP\\PTP.Disk.Tests\\";
    }
}
