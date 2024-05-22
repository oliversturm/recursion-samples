using FSharp.Compiler.Tokenization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Spectre.Console;
using System.Text;
using System.Text.RegularExpressions;

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

  static FSharpTokenizerLexState HighlightFSharpLine(StringBuilder target, FSharpLineTokenizer tokenizer, string line,
    FSharpTokenizerLexState state) {
    (FSharpOption<FSharpTokenInfo> tokenInfo, FSharpTokenizerLexState newState) = tokenizer.ScanToken(state);
    if (FSharpOption<FSharpTokenInfo>.get_IsSome(tokenInfo)) {
      var token = tokenInfo.Value;
      var tokenText = Markup.Escape(line.Substring(token.LeftColumn, token.RightColumn - token.LeftColumn + 1));
      target.Append(token.ColorClass switch {
        FSharpTokenColorKind.Keyword => $"[bold #000080]{tokenText}[/]",
        FSharpTokenColorKind.Number or FSharpTokenColorKind.String => $"[#ff8d32]{tokenText}[/]",
        FSharpTokenColorKind.Identifier => $"[italic]{tokenText}[/]",
        FSharpTokenColorKind.Comment => $"[red italic]{tokenText}[/]",
        //_ => $"<{token.ColorClass}>{tokenText}</>"
        _ => tokenText
      });
      var returnedState = HighlightFSharpLine(target, tokenizer, line, newState);
      // Normally we return returnedState, but if the current token is a comment,
      // lets return to the state we had before then.
      // This is a modification of the recursive algorithm recommended here:
      // https://fsharp.github.io/fsharp-compiler-docs/fcs/tokenizer.html
      // I have tested this with line-end comments // like this
      // and with (*inline comments*) like this
      // and it seems to work okay.
      // However, it does not work correctly with multi-line comments
      // (* like this and
      //   going on here *)
      // ... because the state is not preserved correctly across lines.
      // I think I might leave that as it is for now, but technically it'll be
      // hard to resolve. The recursion doesn't easily allow me to check
      // what happened in a previous iteration (i.e. earlier in the line). And
      // in any case... the problem is that a line ending in (* thing
      // would need to be handled differently from one ending in // thing
      // and I'm not sure how to distinguish the two... each comment is
      // parsed into multiple tokens, separated by spaces basically, and
      // so we may be some iterations deep into the recursion at the end of the line.
      return token.ColorClass == FSharpTokenColorKind.Comment ? state : returnedState;
    }
    else {
      return state;
    }
  }

  static string HighlightFSharpLines(FSharpSourceTokenizer tokenizer, string code) {
    // The proper way to split, so that \r\n doesn't result in extra empty lines
    var lines = Regex.Split(code, "\r\n|\r|\n");
    var builder = new StringBuilder();
    var state = FSharpTokenizerLexState.Initial;
    foreach (string line in lines) {
      var lineTokenizer = tokenizer.CreateLineTokenizer(line);
      state = HighlightFSharpLine(builder, lineTokenizer, line, state);
      builder.Append(Environment.NewLine);
    }

    return builder.ToString();
  }

  static string HighlightFSharp(string code) {
    var defines = ListModule.Empty<string>();
    var tokenizer = new FSharpSourceTokenizer(defines, "input.fs", "PREVIEW", FSharpOption<bool>.None);

    return HighlightFSharpLines(tokenizer, code);
  }

  public static string Highlight(string code, string language) => language switch {
    "cs" => HighlightCSharp(code),
    "fs" => HighlightFSharp(code),
    _ => code
  };
}