namespace ANPaX.IO.interfaces
{
    internal interface ISerializer
    {
        void Serialize<T>(T output, string filename);

        T DeserializeFile<T>(string filename);
    }
}
