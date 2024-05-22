namespace CSSumStep4;

public static class Program {
  static Action<ulong> OutputResult(string prefix) =>
    value => Console.WriteLine($"{prefix}: {value}");

  static ulong SumRec(Span<ulong> l) => l switch {
    [] => 0,
    [var x, .. var xs] => x + SumRec(xs)
  };

  public static void Main() {
    var lotsOfNumbers = Enumerable.Range(1, 300000)
      .Select(i => (ulong)i)
      .ToArray();
    OutputResult("SumRec lotsOfNumbers")(SumRec(lotsOfNumbers));
  }
}
