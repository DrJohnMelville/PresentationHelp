﻿using System.Text.RegularExpressions;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.MessageScreens;

public partial class MessageScreenParser : ICommandParser
{
    public string CommandGroupTitle => "Message\r\n    [Message Content]";

    public IEnumerable<ICommandInfo> Commands => [];

    public ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder) =>
        new(Parser().Match(command) is { Success: true } match ?
            new CommandResult(new MessageScreen(match.Groups[1].Value), CommandResultKind.NewHtml) :
            new CommandResult(holder.Screen, CommandResultKind.NotRecognized));

    [GeneratedRegex(@"^\s*Message\r\n(.+)$", RegexOptions.IgnoreCase)]
    private partial Regex Parser();
}