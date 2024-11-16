﻿using Server.Models;
using Server.Repositories.Base;
using Stripe.Climate;

namespace Server.Repositories
{
    public class ProductsToBuyRepository : Repository<ProductsToBuy, int>
    {
        public ProductsToBuyRepository(FarminhouseContext context) : base(context) { }





    }
}
