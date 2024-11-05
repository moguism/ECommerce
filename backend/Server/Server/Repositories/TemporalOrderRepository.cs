using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class TemporalOrderRepository : Repository<TemporalOrder, int>
    {
        public TemporalOrderRepository(FarminhouseContext context) : base(context) { }

    }
}
