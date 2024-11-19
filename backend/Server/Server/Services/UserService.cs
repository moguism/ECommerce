using Server.Models;

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

    public async Task DeleteUser(User user)
    {
        _unitOfWork.UserRepository.Delete(user);
        await _unitOfWork.SaveAsync();
    }
}
