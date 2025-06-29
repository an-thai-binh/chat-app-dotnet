namespace ChatAppApi.Models
{
    public class Role
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public List<User> Users { get; set; } = new();
        public List<Permission> Permissions { get; set; } = new();
    }
}
