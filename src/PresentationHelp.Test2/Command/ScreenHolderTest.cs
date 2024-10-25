﻿using System.Printing;
using PresentationHelp.Command.Connection;
using PresentationHelp.ScreenInterface;

namespace PresentationHelp.Test2.Command;

public class ScreenHolderTest
{
    private readonly ScreenHolder sut = new(Mock.Of<IScreenParser>());

    [Test]
    [Arguments("~   FontSize   15.2  ")]
    [Arguments("~fontsize15.2")]
    public async Task SetFontSizeTest(string command)
    {
        sut.FontSize.Should().Be(24);
        (await sut.TryParseCommandAsync(command)).Should().BeTrue();
        sut.FontSize.Should().Be(15.2);
    }


}