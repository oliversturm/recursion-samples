namespace CSSumStep5;

public static class Program {
  static Action<ulong> OutputResult(string prefix) =>
    value => Console.WriteLine($"{prefix}: {value}");

  static ulong SumRecTail(Span<ulong> l, ulong result) => l switch {
    [] => result,
    [var x, .. var xs] => SumRecTail(xs, result + x)
  };

  public static void Main() {
    var lotsOfNumbers = Enumerable.Range(1, 300000)
      .Select(i => (ulong)i)
      .ToArray();
    OutputResult("SumRecTail lotsOfNumbers")(SumRecTail(lotsOfNumbers, 0));
  }
}