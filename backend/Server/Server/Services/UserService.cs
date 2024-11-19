using Server.Models;
using System.Collections;

namespace Server.Services;

public class UserService
{
    private readonly UnitOfWork _unitOfWork;

    public UserService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User> GetUserFromDbByStringId(string stringId)
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
}
