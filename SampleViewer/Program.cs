using Spectre.Console;
using System.Diagnostics;
using System.Text.Json;

namespace SampleViewer;

static class Program {
  static void Main(string[] args) {
    string? configFile = null;

    if (args.Length == 0) {
      // check if there's a file "config.json" in the current directory
      if (!File.Exists("config.json")) {
        AnsiConsole.Markup("[red]Please provide a path to a config file as the first parameter.[/]");
        return;
      }
      else {
        configFile = "config.json";
      }
    }
    else {
      configFile = args[0];
    }

    var config = JsonSerializer.Deserialize<List<SampleGroup>>(File.ReadAllText(configFile!),
      new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    if (config == null) {
      AnsiConsole.Markup("[red]Invalid config file[/]");
      return;
    }

    Config.AssignShortcuts(config);

    Sample? currentSample = null;
    bool quit = false;

    do {
      AnsiConsole.Clear();
      if (currentSample == null) {
        (currentSample, quit) = RenderGroupLevel(config);
      }
      else {
        (currentSample, quit) = RenderSample(currentSample!);
      }
    } while (!quit);
  }

  private static (Sample? currentSample, bool quit) RenderSample(Sample currentSample) {
    AnsiConsole.MarkupLine($"[bold]{currentSample.Title}[/]");
    AnsiConsole.WriteLine();
    try {
      AnsiConsole.WriteLine(File.ReadAllText(Path.Combine(currentSample.ProjectPath, currentSample.ViewFile)));
    }
    catch (Exception ex) {
      AnsiConsole.WriteLine(ex.Message);
    }

    AnsiConsole.WriteLine();
    var key = AnsiConsole.Prompt(
      new TextPrompt<string>("[red](R)[/]efresh, [red](V)[/]iew code, E[red](x)[/]ecute, [red](Q)[/]uit")
        .InvalidChoiceMessage("Invalid key")
        .Validate(choice => choice == "q" || choice == "r" || choice == "v" || choice == "x"));
    switch (key) {
      case "r":
        return (currentSample, false);
      case "v":
        Process.Start("rider", Path.Combine(currentSample.ProjectPath, currentSample.ViewFile));
        return (currentSample, false);
      case "x":
        AnsiConsole.WriteLine();
        Process.Start("dotnet", $"run --project {currentSample.ProjectPath}").WaitForExit();
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[red]Press any key to continue[/]");
        Console.ReadKey();
        return (currentSample, false);
      case "q":
        return (null, false);
      default:
        return (currentSample, false);
    }
  }

  private static (Sample? currentSample, bool quit) RenderGroupLevel(List<SampleGroup> config) {
    foreach (var group in config) {
      AnsiConsole.MarkupLine($"[bold]{group.GroupTitle}[/]");
      foreach (var sample in group.Samples) {
        AnsiConsole.MarkupLine($"  [red]({sample.Shortcut.ToString().ToUpper()})[/] {sample.Title}");
      }

      AnsiConsole.WriteLine();
    }

    Sample? currentSample = null;
    bool quit = false;
    var key = AnsiConsole.Prompt(new TextPrompt<string>("Press a key to select a sample, or 'q' to quit")
      .InvalidChoiceMessage("Invalid key")
      .Validate(choice => {
        if (choice == "q") {
          quit = true;
          return true;
        }

        return config.SelectMany(g => g.Samples).Any(s => s.Shortcut.ToString().ToUpper() == choice.ToUpper());
      }));
    currentSample = config.SelectMany(g => g.Samples)
      .FirstOrDefault(s => s.Shortcut.ToString().ToUpper() == key.ToUpper());

    return (currentSample, quit);
  }
}