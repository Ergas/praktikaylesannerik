using System;
using System.Data.SqlClient;
using System.Net;

namespace praktikaylrik
{
    public static class DatabaseConnection
    {
        public static string ConnectionString = GetPath();

        private static string GetPath()
        {
            List<string> findPath = Environment.CurrentDirectory.Split("praktikaylesannerik\\").ToList();
            return @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + findPath[0] + "praktikaylesannerik\\database\\registration_system.mdf;Integrated Security=True;Connect Timeout=30";
        }
    }
}
