namespace Lms.Application;

/// <summary>
/// Delegate used by all Display* methods so the caller decides where the
/// text goes (console, UI, a test collector, ...) instead of the model
/// being hard-coupled to Console.WriteLine.
/// </summary>
public delegate void OutputWriter(string message);
