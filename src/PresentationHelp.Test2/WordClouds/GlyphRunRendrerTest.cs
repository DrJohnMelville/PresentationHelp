using System.Windows;
using PresentationHelp.WordCloud;
using System.Windows.Media;

namespace PresentationHelp.Test2.WordClouds;

public class GlyphRunRendrerTest
{
    [Test]
    public void MeasureCarrier()
    {
        var ff = new FontFamily("Times New Roman");
        var renderedGlyphRun = new GlyphRunRenderer(ff).Render("W", 12, new Point(0, 0)).Measure();
        //renderedGlyphRun.Height.Should().Be(12);
    }
}