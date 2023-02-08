class Program {
  static Action<int> OutputResult(string prefix) => (int value) => Console.WriteLine($"{prefix}: {value}");

  static int SumRec(int[] l) => l switch
  {
    [] => 0,
    [var x, .. var xs] => x + SumRec(xs)
  };

  static int SumRecSeq(IEnumerable<int> s) {
    // using long syntax for clarity
    if (s.Any()) {
      return s.First() + SumRecSeq(s.Skip(1));
    }
    else {
      return 0;
    }
  }

  static int SumRecSeqTail(IEnumerable<int> s, int result = 0) {
    // using long syntax for clarity
    if (s.Any()) {
      return SumRecSeqTail(s.Skip(1), result + s.First());
    }
    else {
      return result;
    }
  }

  static void SumCps(int[] l, Action<int> continuation) {
    switch (l) {
      case []: continuation(0); break;
      case [var x, .. var xs]: SumCps(xs, ix => continuation(ix + x)); break;
    };
  }

  static void SumCpsSeq(IEnumerable<int> s, Action<int> continuation) {
    if (!s.Any()) {
      continuation(0);
      return;
    }

    SumCpsSeq(s.Skip(1), ix => continuation(ix + s.First()));
  }

  public static void Main() {
    // var t = new int[] { 1, 2, 3 };

    // Console.WriteLine($"First: {t.First()}");
    // var t2 = t.Skip(2);
    // if (t2.Any())
    //   Console.WriteLine($"First after skip: {t2.First()}");
    // else
    //   Console.WriteLine("Sequence is now empty");
    // return;

    var l1 = new int[] { 2, 3, 6, 8 };
    OutputResult("SumRec l1")(SumRec(l1));
    SumCps(l1, OutputResult("sumCps l1"));
    OutputResult("SumRecSeq l1")(SumRecSeq(l1));
    OutputResult("SumRecSeqTail l1")(SumRecSeqTail(l1));
    SumCpsSeq(l1, OutputResult("sumCpsSeq l1"));

    var longList = Enumerable.Range(1, 300000).ToArray();

    // This crashes. It also uses 2.5GB memory on the way there,
    // and it takes a very long time. I suspect that the pattern
    // based split is really slow?
    // It runs through something around 258840 iterations, which
    // is a bit surprising.
    //OutputResult("SumRec long")(SumRec(longList));

    // This also crashes, but it does so as quickly as you would
    // expect. Runs for 261361 iterations.
    //OutputResult("SumRecSeq long")(SumRecSeq(longList));

    // This one runs for 174220 iterations before crashing with 
    // a stack overflow
    OutputResult("SumRecSeqTail long")(SumRecSeqTail(longList));

    // This still crashes, after 130185 iterations. Interesting...
    // I think the compiler does not optimize the recursion like
    // the F# compiler does, and apparently there's no tail call
    // optimization here either.
    //SumCps(longList, OutputResult("sumCps long"));

    // Crashes again, this time after 104539 iterations.
    //SumCpsSeq(longList, OutputResult("sumCpsSeq long"));
  }
}
