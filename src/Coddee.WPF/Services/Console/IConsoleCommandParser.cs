namespace Coddee.Services.ApplicationConsole
{
    public interface IConsoleCommandParser
    {
        CommandParseResult ParseCommand(string commandString);
        void RegisterCommands(params ConsoleCommand[] commands);
    }
}