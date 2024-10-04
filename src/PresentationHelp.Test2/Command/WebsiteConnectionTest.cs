﻿using PresentationHelp.Command.Connection;
using PresentationHelp.Shared;

namespace PresentationHelp.Test2.Command;

public class WebsiteConnectionTest
{
    private readonly WebsiteConnection sut = new();

    [Test]
    public void ThrowOnPrematureGet()
    {
        sut.Invoking(i=>i.GetClient()).Should()
            .Throw<InvalidOperationException>();
    }
}