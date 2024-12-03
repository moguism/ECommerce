using System.Globalization;
using System.Text;
using F23.StringSimilarity;
using F23.StringSimilarity.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Services;

public class SmartSearchService
{
    private const double THRESHOLD = 0.75;
    private readonly UnitOfWork _unitOfWork;
    private readonly INormalizedStringSimilarity _stringSimilarityComparer;

    public SmartSearchService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _stringSimilarityComparer = new JaroWinkler();
    }

    public async Task<IEnumerable<Product>> Search(string query, string productType)
    {
        ICollection<Product> products = await _unitOfWork.ProductRepository
            .GetQueryable()
            .Include(product => product.Category)
            .Where(product => product.Category.Name.Equals(productType))
            .ToArrayAsync();

        if (query == null || query.Equals(""))
        {
            return products;
        }

        IEnumerable<Product> matchingProducts = FindMatchingProducts(query, products);

        return matchingProducts;
    }

    public IEnumerable<Product> FindMatchingProducts(string query, IEnumerable<Product> products)
    {
        string[] queryKeys = GetKeys(ClearText(query));
        List<Product> matchingProducts = new List<Product>();

        foreach (var product in products)
        {
            string[] productKeys = GetKeys(ClearText(product.Name));

            if (IsMatch(queryKeys, productKeys))
            {
                matchingProducts.Add(product);
            }
        }

        return matchingProducts;
    }

    private bool IsMatch(string[] queryKeys, string[] productKeys)
    {
        foreach (var queryKey in queryKeys)
        {
            foreach (var productKey in productKeys)
            {
                if (IsMatch(queryKey, productKey))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsMatch(string queryKey, string productKey)
    {
        return queryKey == productKey
            || productKey.Contains(queryKey)
            || _stringSimilarityComparer.Similarity(queryKey, productKey) >= THRESHOLD;
    }

    private string[] GetKeys(string text)
    {
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private string ClearText(string text)
    {
        return RemoveDiacritics(text.ToLower());
    }

    private string RemoveDiacritics(string text)
    {
        string normalizedString = text.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder(normalizedString.Length);

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
