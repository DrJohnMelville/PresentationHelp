﻿using PresentationHelp.ScreenInterface;

namespace PresentationHelp.CommandModels.ErrorScreens;

public class ErrorScreen(IScreenDefinition priorScreen, string error): IScreenDefinition
{
    public string Error { get; } = error;

    public Task AcceptDatum(string user, string datum) => priorScreen.AcceptDatum(user, datum);

    public ValueTask<bool> TryParseCommandAsync(string command) =>
        priorScreen.TryParseCommandAsync(command);

    public string HtmlForUser(IHtmlBuilder builder) => "";
    public bool UserHtmlIsDirty => false;

    public object PublicViewModel => priorScreen.PublicViewModel;
    public object CommandViewModel => new ErrorScreenViewModel(error, priorScreen);
}