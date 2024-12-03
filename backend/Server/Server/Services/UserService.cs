using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Stripe;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Server.Services;

public class UserService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly TokenValidationParameters _tokenParameters;
    private readonly UserMapper _userMapper;

    public UserService(UnitOfWork unitOfWork, IOptionsMonitor<JwtBearerOptions> jwtOptions, UserMapper userMapper)
    {
        _unitOfWork = unitOfWork;
        _userMapper = userMapper;
        _tokenParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme)
                .TokenValidationParameters;
    }

    public async Task<User> GetUserFromDbByStringId(string stringId)
    {

        // Pilla el usuario de la base de datos
        return await _unitOfWork.UserRepository.GetByIdAsync(Int32.Parse(stringId));
    }

    public async Task<User> GetUserFromStringWithTemporal(string stringId)
    {

        // Pilla el usuario de la base de datos
        return await _unitOfWork.UserRepository.GetAllInfoWithTemporal(Int32.Parse(stringId));
    }

    public async Task<User> GetUserAndOrdersFromDbByStringId(string stringId)
    {

        // Pilla el usuario de la base de datos
        return await _unitOfWork.UserRepository.GetAllInfoById(Int32.Parse(stringId));
    }

    public async Task<User> GetUserById(int id)
    {

        // Pilla el usuario de la base de datos
        return await _unitOfWork.UserRepository.GetAllInfoById(id);
    }

    public async Task<IEnumerable<User>> GetAllUsersExceptId(int id)
    {
        return await _unitOfWork.UserRepository.GetAllInfoExceptId(id);
    }

    public async Task DeleteUser(User user)
    {
        _unitOfWork.UserRepository.Delete(user);
        await _unitOfWork.SaveAsync();
    }

    public async Task<User> UpdateUser(User user)
    {
        User updatedUser = _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveAsync();
        return updatedUser;
    }

    public string ObtainToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // EL CONTENIDO DEL JWT
            Claims = new Dictionary<string, object>
                    {
                        { "id", user.Id },
                        { "name", user.Name },
                        { ClaimTypes.Role, user.Role }
                    },
            Expires = DateTime.UtcNow.AddYears(3),
            SigningCredentials = new SigningCredentials(
                    _tokenParameters.IssuerSigningKey,
                    SecurityAlgorithms.HmacSha256Signature
                )
        };
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public UserAfterLoginDto ToDto(User user)
    {
        return _userMapper.ToDto(user);
    }

    public IEnumerable<UserAfterLoginDto> ToDto(IEnumerable<User> users)
    {
        return _userMapper.ToDto(users);
    }

    public async Task<User> InsertUser(User user)
    {
        User newUser = await _unitOfWork.UserRepository.InsertAsync(user);
        await _unitOfWork.SaveAsync();
        return newUser;
    }

    public async Task<User> GetUserByEmailAndPassword(string email, string password)
    {
        User user = await _unitOfWork.UserRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return null;
        }
        PasswordService passwordService = new PasswordService();
        if(passwordService.IsPasswordCorrect(user.Password, password))
        {
            return user;
        }
        else
        {
            return null;
        }
    }

    public async Task<string> RegisterUser(UserSignUpDto receivedUser)
    {
        User user = _userMapper.ToEntity(receivedUser);

        PasswordService passwordService = new PasswordService();
        user.Password = passwordService.Hash(receivedUser.Password);

        user.Role = "User";
        User newUser = await InsertUser(user);
        return ObtainToken(newUser);
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _unitOfWork.UserRepository.GetByIdAsync(id);
    }
        

}
