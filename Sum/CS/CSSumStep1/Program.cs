namespace CSSumStep1;

public static class Program {
  static Action<ulong> OutputResult(string prefix) =>
    value => Console.WriteLine($"{prefix}: {value}");

  static ulong Go(ulong target, ulong x) {
    if (x != target)
      return Go(target, x + 1);
    else return x;
  }

  public static void Main() {
    OutputResult("Go 10")(Go(10, 0));
  }
}