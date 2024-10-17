using PresentationHelp.ScreenInterface;

namespace PresentationHelp.CommandModels.ErrorScreens;

public record ErrorScreenViewModel(string Error, IScreenDefinition Prior);