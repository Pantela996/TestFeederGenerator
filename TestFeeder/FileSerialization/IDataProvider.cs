namespace TestFeeder.FileSerialization
{
    public interface IDataProvider
    {
        void WriteToFile(string filename, object content);

        object ReadFromFile(string filename);
    }
}