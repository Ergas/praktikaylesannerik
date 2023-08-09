namespace praktikaylrik
{
    public static class DatabaseConnection
    {
        public static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + GetPath() + "database\\registration_system.mdf;Integrated Security=True;Connect Timeout=30";

        private static string GetPath()
        {
            List<string> findPath = Environment.CurrentDirectory.Split("\\").ToList();
            findPath.RemoveRange(findPath.Count - 3, 3);
            string result = "";
            foreach (string s in findPath)
            {
                result += s + "\\";
            }
            return result;
        }
    }
}
