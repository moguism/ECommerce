using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class PaymentsTypeRepository : Repository<PaymentsType, int>
{
    public PaymentsTypeRepository(FarminhouseContext context) : base(context) { }
}
