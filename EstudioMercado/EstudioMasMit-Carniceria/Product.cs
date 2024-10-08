using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudioMasMit_Carniceria
{
    class Product
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
            StringBuilder stringBuilder = new StringBuilder(); // Para convertir a cadenas de texto de forma eficiente
            stringBuilder.AppendLine($"Name: {Name}");
            stringBuilder.AppendLine($"Price: {Price} euros");
            return stringBuilder.ToString();

        }
    }
}
