namespace SampleViewer;

// Config file JSON structure
// [
//  { groupTitle: "C# Sum", samples: [
//    { projectPath: "./CSSum/Step1", title: "CSSum Step 1", viewFile: "Program.cs" },
//    { projectPath: "./CSSum/Step2", title: "CSSum Step 2", viewFile: "Program.cs" }
//  ]},
// { groupTitle: "C# PingPong", samples: [
//    { projectPath: "./CSPingPong", title: "CSPingPong", viewFile: "Program.cs" }
//  ]}
// ]

public record Sample(string ProjectPath, string Title, string ViewFile) {
  public char Shortcut { get; set; } = '1';
}

public record SampleGroup(string GroupTitle, List<Sample> Samples);

public static class Config {
  public static void AssignShortcuts(List<SampleGroup> config) {
    // Assign shortcuts to all samples nested in the groups
    // Begin with '1', go to '9', then continue from 'a'
    char shortcut = '1';
    foreach (var group in config) {
      foreach (var sample in group.Samples) {
        sample.Shortcut = shortcut++;
        if (shortcut == ':') shortcut = 'a';
      }
    }
  }
}