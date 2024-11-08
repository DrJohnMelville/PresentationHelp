using System.Windows;
using PresentationHelp.Sentiment;

namespace PresentationHelp.Test2.Sentiments;

public class HorizontalDeltaTest
{
    [Test]
    public void SinglePointTest()
    {
        var sut = new ScatterEngine([1], 2, 10);
        sut.Points().Should().BeEquivalentTo(
            [new Point(10, 1)]);
    }
    [Test]
    public void TwoDistinctPointTest()
    {
        var sut = new ScatterEngine([1, 10], 2, 10);
        sut.Points().Should().BeEquivalentTo(
            [new Point(10, 1), new Point(10, 10)]);
    }
    [Test]
    public void OverlappingPointTest()
    {
        var sut = new ScatterEngine([1, 1.5], 2, 10);
        sut.Points().Should().BeEquivalentTo(
            [new Point(9, 1), new Point(11, 1.5)]);
    }
}