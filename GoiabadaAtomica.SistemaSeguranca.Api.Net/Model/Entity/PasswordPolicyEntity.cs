namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity
{
    public class PasswordPolicyEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int MinLength { get; set; }
        public bool RequiresUppercase { get; set; } = false;
        public bool RequiresLowercase { get; set; } = false;
        public bool RequiresDigit { get; set; } = false;
        public bool RequiresSpecialChar { get; set; } = false;
        public int PasswordHistoryCount { get; set; } = 0;
        public int ExpirationDays { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
