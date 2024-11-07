using Server.Models;

namespace Server.DTOs;

public class ReviewDto
{
    public int Id { get; set; }

    public string Text { get; set; }

    public int Score { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }
}
