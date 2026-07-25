using System.Runtime.CompilerServices;

namespace TestsCommon.Mocks.Helpers;

public class CallTracker
{
    private readonly Dictionary<string, List<object[]>> _calls = new();
    
    public void Record(object[]? args = null, [CallerMemberName] string methodName = "")
    {
        if (!_calls.TryGetValue(methodName, out var list))
        {
            list = new List<object[]>();
            _calls[methodName] = list;
        }

        if (args is null)
            list.Add([]);
        else
            list.Add(args);
    }

    public int GetCallCount(string methodName) => _calls.TryGetValue(methodName, out var list) ? list.Count : 0;

    public int TotalCalls => _calls.Values.Sum(l => l.Count);

    public IReadOnlyList<object[]> GetCallArgs(string methodName) =>
        _calls.TryGetValue(methodName, out var list) ? list : Array.Empty<object[]>();
}