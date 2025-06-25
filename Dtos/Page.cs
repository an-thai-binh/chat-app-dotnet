namespace ChatAppApi.Dtos
{
    public class Page<T>
    {
        public IEnumerable<T> Contents { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int) Math.Ceiling((double)TotalItems / PageSize);

    }
}
