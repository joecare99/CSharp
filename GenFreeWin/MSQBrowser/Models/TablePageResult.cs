using System.Data;

namespace MSQBrowser.Models
{
    public sealed class TablePageResult
    {
        public DataTable Data { get; init; } = new();
        public int Offset { get; init; }
        public int PageSize { get; init; }
        public bool HasMoreRows { get; init; }
    }
}
