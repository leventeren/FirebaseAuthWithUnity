using System.Collections.Generic;
using System.Linq;

public static class FirestoreDebugExt
{
    public static string ToDebugString(this Dictionary<string, object> dict)
    {
        var parts = new List<string>();
        foreach (var kv in dict)
        {
            if (kv.Value is IEnumerable<object> list && !(kv.Value is string))
                parts.Add($"{kv.Key}=[{string.Join(",", list)}]");
            else
                parts.Add($"{kv.Key}={kv.Value}");
        }
        return string.Join(", ", parts);
    }
    public static string DictToString(object obj)
    {
        return obj switch
        {
            Dictionary<string, object> dict => "{" + string.Join(
                ", ",
                dict.Select(kv => $"{kv.Key}={DictToString(kv.Value)}")
            ) + "}",
            List<object> list => "[" + string.Join(", ", list.Select(DictToString)) + "]",
            _ => obj?.ToString() ?? "null"
        };
    }
}