﻿using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Melville.INPC;
using Microsoft.CodeAnalysis;
using PresentationHelp.MessageScreens;
using PresentationHelp.ScreenInterface;
using PresentationHelp.Website.Models.Entities;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Command.Connection;

public partial class ScreenHolder : ICommandParser, IScreenHolder
{
    [AutoNotify] private IScreenDefinition screen =
        new MessageScreen("Internal -- should never show");

    private readonly ICommandParser localProcessor;
    private readonly ICommandParser globalProcessor;
    private  ICommandParser[] targets => [screen, localProcessor, globalProcessor];

    public ScreenHolder(ICommandParser innerParser)
    {
        globalProcessor = innerParser;
        localProcessor = PresenterCommands();
    }

    private ICommandParser PresenterCommands() =>
        new CommandParser()
            .WithCommand($"""~\s*Font\s*Size{ParserParts.RealNumber}""", (double i) => FontSize = i)
            .WithCommand(@"^~\s*Lock\s*Responses", () => ResponsesLocked = true, CommandResultKind.NewHtml)
            .WithCommand(@"^~\s*Allow\s*Responses", () => ResponsesLocked = false, CommandResultKind.NewHtml)
            .WithCommand($$"""^~\s*Display\s*Margin{{ParserParts.RealNumber}}{4}""", 
                (double l, double t, double r, double b) => RelativeThickness = new Thickness(l, t, r, b))
            .WithCommand($$"""^~\s*Display\s*Margin{{ParserParts.RealNumber}}{2}""", 
                (double h, double v) => RelativeThickness = new Thickness(h,v,h,v))
            .WithCommand($"""^~\s*Display\s*Margin{ParserParts.RealNumber}""", (double w) =>
                RelativeThickness = new Thickness(w))
        ;
    #region Command Parsing
    public async ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder)
    {
        foreach (var target in targets)
        {
            if (await target.TryParseCommandAsync(command, holder) is
                { Result: not CommandResultKind.NotRecognized } result)
            {
                Screen = result.NewScreen;
                return result;
            }
        }
        return new(screen, CommandResultKind.NotRecognized);
    }

    [AutoNotify] private bool responsesLocked;
    public Task AcceptDatum(string user, string datum) =>
        ResponsesLocked ? Task.CompletedTask : Screen.AcceptDatum(user, datum);


    public string HtmlForUser(HtmlBuilder htmlBuilder) => ResponsesLocked ? 
        htmlBuilder.CommonClientPage("", "<h2>Responses are currently locked.</h2>") : 
        screen.HtmlForUser(htmlBuilder);

    #endregion

    #region Font Size

    [AutoNotify] private double fontSize = 24;
    [AutoNotify] private string selectedFontSize = "24";
    [AutoNotify] public string FontSizeCommand =>
        FontSize.ToString(CultureInfo.InvariantCulture).Equals(SelectedFontSize) ?
            "" :
            $"~FontSize {SelectedFontSize}";

    #endregion

    #region Display Location

    public void SizeChanged1(FrameworkElement element)
    {
        ActualWidth = element.ActualWidth;
        ActualHeight = element.ActualHeight;
    }

    [AutoNotify] private double actualWidth = 1;
    [AutoNotify] private double actualHeight = 1;
    [AutoNotify] private Thickness relativeThickness = new Thickness(5);
    [AutoNotify] public Thickness Location => new Thickness(
        RelativeThickness.Left*ActualWidth / 100,
        RelativeThickness.Top*ActualHeight / 100,
        RelativeThickness.Right*ActualWidth / 100,
        RelativeThickness.Bottom*ActualHeight / 100);

    #endregion
}