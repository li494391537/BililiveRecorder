using CommandLine;

namespace ConsoleApp1
{
    public class CommandLineOption
    {
        [Option('i', "id", Default = 0, HelpText = "room id", Required = false)]
        public int RoomID { get; set; }
    }
}