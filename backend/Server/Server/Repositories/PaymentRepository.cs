using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class PaymentRepository : Repository<Payment, int>
{
    public PaymentRepository(FarminhouseContext context) : base(context) { }
}
