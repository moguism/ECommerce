using System.Text;

namespace EstudioMercado;

internal class Product
{
    // EL "init" ESPECIFICA QUE SÓLO SE PUEDE ESCRIBIR UNA VEZ, AL INICIO DE LA CLASE EN EL CONSTRUCTOR A
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
