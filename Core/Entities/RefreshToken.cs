namespace Core.Entities
{
    public class RefreshToken : BaseEntity
    {

        public int UserId { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? RevokeDate { get; set; }

        public bool IsActive
        {
            get
            {
                return this.RevokeDate == null && !this.IsExpired;
            }
        }

        public bool IsExpired
        {
            get
            {
                return DateTime.UtcNow >= this.ExpirationDate;
            }
        }

    }
}
