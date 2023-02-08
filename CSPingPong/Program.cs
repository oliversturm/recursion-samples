class Program {
  static int Ping(int c, int v) {
    return c == 0 ? v : Pong(c - 1, v + c);
  }

  static int Pong(int c, int v) {
    return c == 0 ? v : Ping(c - 1, v + c);
  }

  public static void Main() {
    Console.WriteLine($"Ping/pong result: {Ping(300000, 0)}");
  }
}
