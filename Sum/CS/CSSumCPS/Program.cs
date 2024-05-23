namespace CSSumCPS;

public static class Program {
  static Action<ulong> OutputResult(string prefix) =>
    value => Console.WriteLine($"{prefix}: {value}");

  // I tested a few variations, but the JIT does not agree that this implementation
  // is tail-recursive. Unfortunately I don't know how to find out why.
  static void SumCps(Span<ulong> l, Action<ulong> continuation) {
    switch (l) {
      case []:
        continuation(0);
        return;
      case [var x, .. var xs]:
        SumCps(xs, ix => continuation(ix + x));
        return;
    }
  }

  public static void Main() {
    var lotsOfNumbers = Enumerable.Range(1, 300000)
      .Select(i => (ulong)i)
      .ToArray();

    SumCps(lotsOfNumbers, OutputResult("SumCps"));
  }
}