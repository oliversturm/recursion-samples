using System.Diagnostics.CodeAnalysis;

class Program {
  // Result output helper
  static Action<ulong> OutputResult(string prefix) => (ulong value) => Console.WriteLine($"{prefix}: {value}");

  // Sample 1 and 2 -- just iterate to goal, recursively
  static ulong Go(ulong target, ulong x) {
    if (x != target)
      return Go(target, x + 1);
    else return x;
  }

  // Sample 3 -- calculate the sum of values in a list,
  // using a recursive algorithm. Unusual in C#, but common in 
  // other languages! 

  static ulong SumRec(ulong[] l) => l switch
  {
    [] => 0,
    [var x, .. var xs] => x + SumRec(xs)
  };

  // Sample 4 -- same as (3), but using sequences instead of 
  // arrays with the pattern matching syntax.
  static ulong SumRecSeq(IEnumerable<ulong> s) {
    // using long syntax for clarity
    if (s.Any()) {
      return s.First() + SumRecSeq(s.Skip(1));
    }
    else {
      return 0;
    }
  }

  // Sample 5 -- now the result is passed along as a parameter,
  // and the calculation is performed *before* the call.
  // It is no longer necessary to return to the same call instance
  // later to complete the calculation.
  static ulong SumRecSeqTail(IEnumerable<ulong> s, ulong result = 0) {
    // using long syntax for clarity
    if (s.Any()) {
      return SumRecSeqTail(s.Skip(1), result + s.First());
    }
    else {
      return result;
    }
  }

  // Sample 6 -- to prevent any performance issues due to the list handling
  // (which is a bit contrived in the examples above, especially for C#),
  // here is a variation of the last function that still calculates a sum
  // in a very similar way, but does not use a list.
  static ulong SumRecMaxTail(ulong max, ulong val, ulong result = 0) {
    if (val < max)
      return SumRecMaxTail(max, val + 1, result + val);
    else
      return result;
  }

  // // Encapsulation of the either/or result 
  // // Not needed in dynamic languages
  // public class TrampolineResult<Tout> {
  //   public TrampolineResult(Func<TrampolineResult<Tout>> continuation) {
  //     IsContinuation = true;
  //     this.Continuation = continuation;
  //     this.Result = default!; // satisfy compiler
  //   }
  //   public bool IsContinuation { get; }
  //   public Func<TrampolineResult<Tout>> Continuation { get; }

  //   public TrampolineResult(Tout result) {
  //     IsContinuation = false;
  //     this.Result = result;
  //     this.Continuation = default!; // satisfy compiler
  //   }
  //   public Tout Result { get; }
  // }

  // // Syntactic convenience
  // public class TrampolineResult {
  //   public static TrampolineResult<Tout> From<Tout>(Tout result) => new(result);
  //   public static TrampolineResult<Tout> From<Tout>(Func<TrampolineResult<Tout>> continuation) => new(continuation);
  // }

  // public static Tout Trampoline<Tout>(Func<TrampolineResult<Tout>> f) {
  //   var currentResult = new TrampolineResult<Tout>(f);
  //   while (currentResult.IsContinuation)
  //     currentResult = currentResult.Continuation.Invoke();
  //   return currentResult.Result;
  // }

  // static TrampolineResult<ulong> SumTrampoline(ulong x, ulong current = 0) {
  //   if (x == 0)
  //     return TrampolineResult.From(current);
  //   else
  //     return TrampolineResult.From(() => SumTrampoline(x - 1, current + x));
  // }

  // static void SumCps(ulong[] l, Action<ulong> continuation) {
  //   switch (l) {
  //     case []: continuation(0); break;
  //     case [var x, .. var xs]: SumCps(xs, ix => continuation(ix + x)); break;
  //   };
  // }

  // static void SumCpsSeq(IEnumerable<ulong> s, Action<ulong> continuation) {
  //   if (!s.Any()) {
  //     continuation(0);
  //     return;
  //   }

  //   SumCpsSeq(s.Skip(1), ix => continuation(ix + s.First()));
  // }

  // static IEnumerable<ulong> Range(ulong start, ulong count) {
  //   for (ulong x = start; x < start + count; x++) {
  //     yield return x;
  //   }
  // }


  public static void Main() {
    // Sample 1 -- iterate to goal
    OutputResult("Go 10")(Go(10, 0));

    // Sample 2 -- larger goal
    //OutputResult("Go 300000")(Go(300000, 0));

    // Sample 3 -- calculate sum of numbers in a list
    var l1 = new ulong[] { 2, 3, 6, 8 };
    OutputResult("SumRec l1")(SumRec(l1));

    // Sample 4 -- same as (3), but using an IEnumerable sequence
    OutputResult("SumRecSeq l1")(SumRecSeq(l1));

    // Sample 5 -- similar to (4), but now calculating *before* the call
    OutputResult("SumRecSeqTail l1")(SumRecSeqTail(l1));

    // Sample 6 -- using the list-less variation of the function
    // for testing purposes
    OutputResult("SumRecMaxTail 20")(SumRecMaxTail(20, 0));

    // Sample 7 -- now for a real test: 300000
    OutputResult("SumRecMaxTail 300000")(SumRecMaxTail(300000, 0));


    // OutputResult("SumTrampoline 100")(Trampoline(() => SumTrampoline(100)));

    // //var longList = Enumerable.Range(1, 300000).ToArray();

    // // This crashes. It also uses 2.5GB memory on the way there,
    // // and it takes a very long time. I suspect that the pattern
    // // based split is really slow?
    // // It runs through something around 258840 iterations, which
    // // is a bit surprising.
    // //OutputResult("SumRec long")(SumRec(longList));

    // // This also crashes, but it does so as quickly as you would
    // // expect. Runs for 261361 iterations.
    // //OutputResult("SumRecSeq long")(SumRecSeq(longList));

    // // This one runs for 174220 iterations before crashing with 
    // // a stack overflow
    // //OutputResult("SumRecSeqTail long")(SumRecSeqTail(Range(1, 300000)));

    // // This still crashes, after 130185 iterations. Interesting...
    // // I think the compiler does not optimize the recursion like
    // // the F# compiler does, and apparently there's no tail call
    // // optimization here either.
    // //SumCps(longList, OutputResult("sumCps long"));

    // // Crashes again, this time after 104539 iterations.
    // //SumCpsSeq(longList, OutputResult("sumCpsSeq long"));

    // OutputResult("SumTrampoline 300000")(Trampoline(() => SumTrampoline(300000)));

  }
}
