namespace CSSumTrampolining;

public static class Program {
  static Action<ulong> OutputResult(string prefix) =>
    value => Console.WriteLine($"{prefix}: {value}");

  public class TrampolineResult<Tout> {
    public TrampolineResult(Func<TrampolineResult<Tout>> continuation) {
      IsContinuation = true;
      this.Continuation = continuation;
      this.Result = default!; // satisfy compiler
    }

    public bool IsContinuation { get; }
    public Func<TrampolineResult<Tout>> Continuation { get; }

    public TrampolineResult(Tout result) {
      IsContinuation = false;
      this.Result = result;
      this.Continuation = default!; // satisfy compiler
    }

    public Tout Result { get; }
  }

// Syntactic convenience -- maybe OOP people like this syntax?
  public class TrampolineResult {
    public static TrampolineResult<Tout> From<Tout>(Tout result) => new(result);
    public static TrampolineResult<Tout> From<Tout>(Func<TrampolineResult<Tout>> continuation) => new(continuation);
  }

// Or maybe like this for a more FP feel?
  static TrampolineResult<Tout> Tr<Tout>(Tout result) => new(result);
  static TrampolineResult<Tout> Tr<Tout>(Func<TrampolineResult<Tout>> continuation) => new(continuation);

  static Tout Trampoline<Tout>(Func<TrampolineResult<Tout>> f) {
    var currentResult = new TrampolineResult<Tout>(f);
    while (currentResult.IsContinuation)
      currentResult = currentResult.Continuation.Invoke();
    return currentResult.Result;
  }

  // Error CS8175:
  // Cannot use local variable 'xs' of byref-like type 'Span<ulong>' inside lambda expression
  // As mentioned elsewhere, being able to pattern-match on Span<T> does not solve
  // all problems.
  // static TrampolineResult<ulong> SumTrampoline(Span<ulong> l, ulong result = 0) {
  //   return l switch {
  //     [] => Tr(result),
  //     [var x, .. var xs] => Tr(() => SumTrampoline(xs, result + x))
  //   };
  // }

  // Use IEnumerable<ulong> instead of Span<ulong> to avoid CS8175 -- no pattern matching.
  static TrampolineResult<ulong> SumTrampoline(IEnumerable<ulong> l, ulong result = 0) {
    if (!l.Any())
      return Tr(result);
    else
      return Tr(() => SumTrampoline(l.Skip(1), result + l.First()));
  }

  public static void Main() {
    var lotsOfNumbers = Enumerable.Range(1, 300000)
      .Select(i => (ulong)i)
      .ToArray();

    OutputResult("SumTrampoline")(Trampoline(() => SumTrampoline(lotsOfNumbers)));
  }
}