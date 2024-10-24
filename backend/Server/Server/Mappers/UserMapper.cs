using Server.DTOs;
using Server.Models;

namespace Server.Mappers
{
    public class UserMapper
    {

        //Pasar de usuario a dto
        public UserAfterLoginDto ToDto(User user)
        {
            return new UserAfterLoginDto
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
        public IEnumerable<UserAfterLoginDto> ToDto(IEnumerable<User> users)
        {
            return users.Select(ToDto);
        }


        //Pasar de Dto a usuario
        public User ToEntity(UserAfterLoginDto userDto)
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

        public User ToEntity(UserSignUpDto userDto)
        {
            return new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,
                Address = userDto.Address,
                Role = userDto.Role,
            };
        }


        //Pasar la lista de dtos a usuarios
        public IEnumerable<User> ToEntity(IEnumerable<UserAfterLoginDto> usersDto)
        {
            return usersDto.Select(ToEntity);
        }

        public IEnumerable<User> ToEntity(IEnumerable<UserSignUpDto> usersDto)
        {
            return usersDto.Select(ToEntity);
        }

    }
}
