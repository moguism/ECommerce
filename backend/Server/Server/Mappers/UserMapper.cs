using Server.Models;

namespace Server.Mappers
{
    public class UserMapper
    {

        public UserDto ToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Address = user.Address,
                Orders = user.Orders,
                Reviews = user.Reviews,
            }
        
        }


    }
}
