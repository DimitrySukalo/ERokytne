using System.Collections.ObjectModel;

namespace ERokytne.Application.Telegram.Models;

public class StreamCollection : Collection<Stream>, IDisposable
{
    public void Dispose()
    {
        foreach (var stream in this) {
            stream.Dispose();
        }
    }
}