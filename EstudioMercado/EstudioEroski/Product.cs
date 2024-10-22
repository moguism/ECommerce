using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudioEroski
{
    internal class Product
    {
        public string Name { get; init; }
        public decimal Price { get; init; }
        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Name: {Name}");
            stringBuilder.Append($"price: {Price} euros");
            return stringBuilder.ToString();
        }
    }
}
