namespace Server.Models;

public class ExternalLogin
    {
        public Guid Id { get; set; }
        public string LoginProvider { get; set; } // e.g., "Google", "Facebook", "GitHub"
        public string ProviderKey { get; set; }   // Unique ID from the provider
        public string ProviderDisplayName { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }