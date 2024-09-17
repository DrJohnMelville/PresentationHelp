using Melville.INPC;

namespace PresentationHelp.Website.Models.Entities;

public partial class Meeting
{
    [FromConstructor] public string Name { get; }
    public DateTimeOffset ExpiresAt { get; set; }
}