using System.Diagnostics;
using System.Windows;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PresentationHelp.Sentiment;

public readonly struct ScatterEngine
{
    private readonly double[] ys;
    private readonly double diameterSquared;
    private readonly double diameter;
    private readonly double[] xs;

    public double Bottom => ys[0];
    public double Top => ys[^1];
    public double BoxBottom => ys[Quartile(1)];
    public double Median => ys[Quartile(2)];
    public double BoxTop => ys[Quartile(3)];

    private int Quartile(int quartile) => quartile * ys.Length / 4;

    public ScatterEngine(double[] ys, double diameter, double center)
    {
        Debug.Assert(ys.Length > 0);
        this.ys = ys;
        this.diameter = diameter;
        this.diameterSquared = diameter*diameter;
        Array.Sort(ys);
        xs = new double[ys.Length];
        ComputeXs(center);
    }

    private void ComputeXs(double center)
    {
        int minimum = 0;
        double maxOffset = 0;
        for (int i = 0; i < ys.Length; i++)
        {
            minimum = FindFirstCandidateCollider(minimum, i);
            for (int j = minimum; j < i; j++)
            {
                TryAdjustOffset(j, i);
            }
            maxOffset = Math.Max(maxOffset, xs[i]);
        }

        FixupFinalOffsets(xs, center - (maxOffset / 2));
    }

    private int FindFirstCandidateCollider(int minimum, int i)
    {
        for (; minimum < i && DeltaYMoreThanDiameter(i, minimum); minimum++) { }
        return minimum;
    }


    private bool DeltaYMoreThanDiameter(int i, int minimum) => ys[i] - ys[minimum] > diameter;


    private void TryAdjustOffset(int j, int i)
    {
        if (Overlaps(j, i)) xs[i] = xs[j] + diameter;
    }

    private bool Overlaps(int j, int i) => 
        (Square(xs[j] - xs[i]) + Square(ys[j] - ys[i])) < diameterSquared;

    private double Square(double i) => i * i;


    private static void FixupFinalOffsets(double[] ret, double delta)
    {
        for (int i = 0; i < ret.Length; i++) ret[i] += delta;
    }

    public IEnumerable<Point> Points() => xs.Zip(ys, (x, y) => new Point(x, y));
}