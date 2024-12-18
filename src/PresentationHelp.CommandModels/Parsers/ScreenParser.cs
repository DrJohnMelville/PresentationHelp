﻿using PresentationHelp.CommandModels.ErrorScreens;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.CommandModels.Parsers;

public class ScreenParser(IList<ICommandParser> parsers): ICommandParser
{
    public async ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder)
    {
        foreach (var parser in parsers)
        {
            if ((await parser.TryParseCommandAsync(command, holder)) is 
                {Result: not CommandResultKind.NotRecognized} screen)
                return screen;
        }
        return new CommandResult(new ErrorScreen(holder.Screen, $"""
            Could not parse the command:
            {command}
            """), CommandResultKind.KeepHtml);
    }

    public string CommandGroupTitle => "Create Screens";

    public IEnumerable<ICommandInfo> Commands => parsers;
}