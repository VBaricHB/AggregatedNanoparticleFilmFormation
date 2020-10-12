namespace ANPaX.IO.DBConnection.Db
{
    public class ConnectionData
    {
        public string SQLConnectionString { get; set; } = "Default";
        public string SQLiteFilePath { get; set; } = "ANPaXDB.sqlite";
        public string SQLiteConnectionString => $"Data Source={SQLiteFilePath}";
    }
}
