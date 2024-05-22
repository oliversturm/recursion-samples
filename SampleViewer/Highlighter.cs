using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Spectre.Console;
using System.Text;

namespace SampleViewer;

public static class Highlighter {
  static void AppendTrivia(StringBuilder builder, SyntaxTriviaList triviaList) {
    foreach (var trivia in triviaList) {
      var escapedTrivia = Markup.Escape(trivia.ToString());
      builder.Append(trivia.Kind() switch {
        SyntaxKind.SingleLineCommentTrivia or
          SyntaxKind.MultiLineCommentTrivia => $"[red italic]{escapedTrivia}[/]",
        _ => escapedTrivia
      });
    }
  }

  static string HighlightCSharpTokenWithSymbol(SyntaxToken token, ISymbol? symbol) {
    var escapedToken = Markup.Escape(token.ToString());

    if (token.IsKeyword()) {
      return $"[bold #000080]{escapedToken}[/]";
    }
    else if (symbol?.Kind == SymbolKind.NamedType) {
      return $"[#1681bb]{Markup.Escape(token.ToString())}[/]";
    }
    else if (token.IsKind(SyntaxKind.IdentifierToken) && symbol?.Kind == SymbolKind.Method) {
      return $"[italic]{token}[/]";
    }
    else {
      return token.Kind() switch {
        SyntaxKind.IdentifierToken => escapedToken,
        SyntaxKind.NumericLiteralToken or
          SyntaxKind.StringLiteralToken or
          SyntaxKind.InterpolatedStringStartToken or
          SyntaxKind.InterpolatedStringTextToken or
          SyntaxKind.InterpolatedStringEndToken => $"[#ff8d32]{escapedToken}[/]",
        _ => escapedToken
      };
    }
  }

  static string HighlightCSharp(string code) {
    var tree = CSharpSyntaxTree.ParseText(code);
    var compilation = CSharpCompilation.Create("Highlighting")
      .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
      .AddSyntaxTrees(tree);
    var semanticModel = compilation.GetSemanticModel(tree);
    var root = tree.GetRoot();

    var builder = new StringBuilder();
    foreach (var token in root.DescendantTokens()) {
      AppendTrivia(builder, token.LeadingTrivia);
      SymbolInfo? symbolInfo = token.Parent != null ? semanticModel.GetSymbolInfo(token.Parent) : null;
      var symbol = symbolInfo?.Symbol;
      builder.Append(HighlightCSharpTokenWithSymbol(token, symbol));
      AppendTrivia(builder, token.TrailingTrivia);
    }

    return builder.ToString();
  }

  static string HighlightFSharp(string code) {
    return code;
  }

  public static string Highlight(string code, string language) => language switch {
    "cs" => HighlightCSharp(code),
    "fs" => HighlightFSharp(code),
    _ => code
  };
}