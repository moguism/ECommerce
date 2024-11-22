using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

    public UserService(UnitOfWork unitOfWork, IOptionsMonitor<JwtBearerOptions> jwtOptions)
    {
        _unitOfWork = unitOfWork;
        _tokenParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme)
                .TokenValidationParameters;
    }

    public async Task<User> GetUserFromDbByStringId(string stringId)
    {

        // Pilla el usuario de la base de datos
        return await _unitOfWork.UserRepository.GetOnlyOrdersById(Int32.Parse(stringId));
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
                        { ClaimTypes.Role, "Admin" } // TODO: CAMBIAR ESTO
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

    public async Task InsertUser(User user)
    {
        await _unitOfWork.UserRepository.InsertAsync(user);
        await _unitOfWork.SaveAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        User user = await _unitOfWork.UserRepository.GetByEmailAsync(email);
        return user;
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _unitOfWork.UserRepository.GetByIdAsync(id);
    }
        

}
