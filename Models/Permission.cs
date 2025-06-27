namespace ChatAppApi.Models
{
    public class RevokatedToken
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public List<Role> Roles { get; set; } = new();
    }
}
