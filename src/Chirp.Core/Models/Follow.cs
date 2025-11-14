namespace Chirp.Core.Models;

public class Follow
    {
    public int Id { get; set; }

    public string FollowerId { get; set; } = null!;
    public Author Follower { get; set; } = null!;

    public string FolloweeId { get; set; } = null!;
    public Author Followee { get; set; } = null!;
}
