namespace SzyfryBlokowe
{
    class Program
    {
        static void Main(string[] args)
        {
            new Classes.Common.AppManager(Enums.AppLevel.Dev).Start();
        }
    }
}
