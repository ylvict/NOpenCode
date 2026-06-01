using NOpenCode;

// Simplest possible usage — one line, one question, one answer.
// NOpenCode auto-discovers or starts opencode serve for you.
var answer = await OpenCode.Ask(
    "Write a one-sentence explanation of dependency injection."
);

Console.WriteLine(answer);
