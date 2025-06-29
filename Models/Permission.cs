namespace ChatAppApi.Models
{
    public class Permission
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public List<Role> Roles { get; set; } = new();
    }
}
