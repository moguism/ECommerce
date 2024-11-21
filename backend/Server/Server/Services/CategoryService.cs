using Server.Models;
using Server.Repositories;

namespace Server.Services;

public class CategoryService
{
    private readonly UnitOfWork _unitOfWork;
    public CategoryService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Category> GetByName(string name)
    {
        switch(name)
        {
            case "Frutas":
                name = "fruits";
                break;
            case "Verduras":
                name = "vegetables";
                break;
            case "Carne":
                name = "meat";
                break;
            default:
                return null;
        }
        Category category = await _unitOfWork.CategoryRepository.GetByName(name);
        return category;
    }
}
