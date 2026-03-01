using System.Diagnostics.CodeAnalysis;

namespace Fl.Event.Handling.Extensions.Tests;

[ExcludeFromCodeCoverage]
internal class Payload2(int id)
{
    public int Id { get; } = id;
    public override string ToString() => $"{GetType().Name}: id={Id}";
}
