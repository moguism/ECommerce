using Server.Models;

namespace Server.Mappers
{
    public class UserMapper
    {

        //Pasar de usuario a dto
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
            };

        }


        //Pasar la lista de usuarios a dtos
        public IEnumerable<UserDto> ToDto(IEnumerable<User> users)
        {
            return users.Select(ToDto);

        }


        //Pasar de Dto a usuario
        public User ToEntity(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Name = userDto.Name,
                Email = userDto.Email,
                Role = userDto.Role,
                Address = userDto.Address,
                Orders = userDto.Orders,
                Reviews = userDto.Reviews,
            };

        }


        //Pasar la lista de dtos a usuarios
        public IEnumerable<User> ToEntity(IEnumerable<UserDto> usersDto)
        {
            return usersDto.Select(ToEntity);

        }


    }
}
