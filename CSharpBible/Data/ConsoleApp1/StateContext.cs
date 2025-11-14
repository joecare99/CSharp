using System.Collections.Generic;

namespace ConsoleApp1;

// 2. StateContext für die Umgebung der Simulation
public class StateContext
{
    public string CurrentState { get; set; }
    public Dictionary<string, object> Data { get; } = new();

    public void AddData(string key, object value)
    {
        Data[key] = value;
    }

    public T GetData<T>(string key)
    {
        if (Data.TryGetValue(key, out var val) && val is T tVal)
        {
            return tVal;
        }
        return default(T);
    }
}
