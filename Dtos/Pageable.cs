namespace ChatAppApi.Dtos
{
    public class Pageable
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;
        public string Sort { get; set; } = "id";
        public string Direction { get; set; } = "desc";
    }
}
