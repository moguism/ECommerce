using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class ReviewRepository : Repository<Review, int>
{
    public ReviewRepository(FarminhouseContext context) : base(context) { }
}
