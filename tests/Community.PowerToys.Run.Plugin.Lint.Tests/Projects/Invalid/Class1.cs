using LazyCache;
using Newtonsoft.Json.Linq;

namespace Invalid;

public class Class1
{
    public IAppCache AppCache { get; set; } = null!;
    public JObject Json { get; set; } = null!;
}
