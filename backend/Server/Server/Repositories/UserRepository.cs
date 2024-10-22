using Server.Repositories.Base;
using Server.Models;

namespace Server.Repositories
{
    public class UserRepository :Repository<User,int>
    {
        public UserRepository (FarminhouseContext context) : base(context) { }
    }
}
