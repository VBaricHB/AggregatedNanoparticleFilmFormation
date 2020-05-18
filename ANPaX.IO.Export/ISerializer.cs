namespace ANPaX.IO.Export
{
    internal interface ISerializer
    {
        void Serialize<T>(T output, string filename);
    }
}
