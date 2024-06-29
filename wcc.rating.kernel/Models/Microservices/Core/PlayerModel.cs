namespace wcc.rating.kernel.Models.Microservices.Core
{
    internal class PlayerModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public string? UserId { get; set; }
        public string? Token { get; set; }
        public int Games { get; set; }
        public int Wins { get; set; }
    }
}
