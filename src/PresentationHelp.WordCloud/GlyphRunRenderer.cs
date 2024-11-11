using System.Windows;
using System.Windows.Media;
using KnowledgePicker.WordCloud.Primitives;

namespace PresentationHelp.WordCloud;

public readonly struct GlyphRunRenderer
{
    private readonly GlyphTypeface glyphTypeface;

    public GlyphRunRenderer(FontFamily font)
    {
        if (!TypefaceFromFont(font).TryGetGlyphTypeface(out glyphTypeface))
        {
            throw new ArgumentException("No glyph type face found");
        }
    }

    private static Typeface TypefaceFromFont(FontFamily font) => 
        new(font, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

    public RenderedGlyphRun Render(string text, double size, Point origin)
    {
        var glyphs = new ushort[text.Length];
        var widths = new double[text.Length];
        for (int i = 0; i < text.Length; i++)
        {
            glyphs[i] = glyphTypeface.CharacterToGlyphMap[text[i]];
            widths[i] = glyphTypeface.AdvanceWidths[glyphs[i]] * size;
        }

        var glyphRun = new GlyphRun(
            glyphTypeface, 0, false, size, 1, glyphs, origin, widths,
            null, null, null, null, null, null);

        return new RenderedGlyphRun(glyphRun, size);
    }
}

public readonly struct RenderedGlyphRun(GlyphRun run, double size)
{
    public RectangleD Measure()
    {
        return new RectangleD(run.BaselineOrigin.X, run.BaselineOrigin.Y,
            run.AdvanceWidths.Sum(), run.GlyphTypeface.Height * size);
    }

   public void Draw(DrawingContext dc, Brush brush)
    {
        dc.DrawGlyphRun(brush, run);
    }
}