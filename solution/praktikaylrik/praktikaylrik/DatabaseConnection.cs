using System;
using System.Data.SqlClient;
using System.Net;

namespace praktikaylrik
{
    public static class DatabaseConnection
    {
        public static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + GetPath() + ";Integrated Security=True;Connect Timeout=30";

        private static string GetPath()
        {
            List<string> findPath = Environment.CurrentDirectory.Split("praktikaylesannerik\\").ToList();
            return findPath[0] + "praktikaylesannerik\\database\\registration_system.mdf";
        }
    }
}
