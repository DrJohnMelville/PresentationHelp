using System.Text.RegularExpressions;
using System.Windows.Input;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;

namespace PresentationHelp.Poll;

public partial class PollScreenParser(Func<TimeSpan, Func<ValueTask>, IThrottle> throttleFactory) :
    MultiOptionScreenParser("Poll")
{
    protected override IScreenDefinition CreateScreen(IScreenHolder holder, string[] strings) => 
        new PollScreen(strings, throttleFactory);
}

