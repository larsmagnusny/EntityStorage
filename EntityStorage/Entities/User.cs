using EntityStorage.Attributes;

namespace EntityStorage.CLI.Entities
{
    public record User
    {
        [Key(AutoIncrement = true)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
