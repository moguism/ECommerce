using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SmartSearchController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly SmartSearchService _smartSearchService;

    public SmartSearchController(UnitOfWork unitOfWork, SmartSearchService smartSearchService)
    {
        _unitOfWork = unitOfWork;
        _smartSearchService = smartSearchService;
    }

    [HttpGet]
    public async Task<IEnumerable<Product>> Search([FromQuery] string query)
    {
        ICollection<Product> products = await _unitOfWork.ProductRepository.GetAllAsync();
        IEnumerable<string> results = _smartSearchService.Search(query);

        List<Product> sendProducts = new List<Product>();

        foreach (Product product in products)
        {
            if(results.Contains(product.Name))
            {
                sendProducts.Add(product);
            }
        }

        return sendProducts;
    }
}
