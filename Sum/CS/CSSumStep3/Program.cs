namespace CSSumStep3;

public static class Program {
  static Action<ulong> OutputResult(string prefix) =>
    value => Console.WriteLine($"{prefix}: {value}");

  static ulong SumRec(Span<ulong> l) => l switch {
    [] => 0,
    [var x, .. var xs] => x + SumRec(xs)
  };

  public static void Main() {
    var l = new ulong[] { 2, 3, 6, 8 };
    OutputResult("SumRec l")(SumRec(l));
  }
}