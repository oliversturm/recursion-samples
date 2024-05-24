namespace CSSumCPS;

public static class Program {
  // Return an int this time, just in an effort to make the structure
  // exactly like the one in F# (other than the return type).
  static Func<ulong, int> OutputResult(string prefix) =>
    value => {
      Console.WriteLine($"{prefix}: {value}");
      return 0;
    };

  // I tested a few variations, but the JIT does not agree that this implementation
  // is tail-recursive.
  static int SumCps(Span<ulong> l, Func<ulong, int> continuation) =>
    l switch {
      [] => continuation(0),
      [var x, .. var xs] => SumCps(xs, ix => continuation(ix + x))
    };

  public static void Main() {
    var lotsOfNumbers = Enumerable.Range(1, 300000)
      .Select(i => (ulong)i)
      .ToArray();

    SumCps(lotsOfNumbers, OutputResult("SumCps"));
  }
}