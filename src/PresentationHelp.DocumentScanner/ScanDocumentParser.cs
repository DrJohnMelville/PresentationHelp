using PresentationHelp.ScreenInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PresentationHelp.DocumentScanner;

public partial class ScanDocumentParser : ICommandParser
{
    public string CommandGroupTitle => "ScanDocument";

    public IEnumerable<ICommandInfo> Commands => [];

    public ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder) =>
         ValueTask.FromResult(
             Parser().IsMatch(command) ?
             new CommandResult(new ScanDocumentViewModel(), CommandResultKind.NewHtml) :
             new CommandResult(holder.Screen, CommandResultKind.NotRecognized));

    [GeneratedRegex(@"^\s*ScanDocument$", RegexOptions.IgnoreCase)]
    private static partial Regex Parser();
}
