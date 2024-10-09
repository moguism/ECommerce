using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstudioLaFruteria {
    internal class Product {

        public string Name { get; init; }
        public string Url { get; init; }
        public decimal Price { get; init; }
        public Product(string name, string url, decimal price) {
            Name = name;
            Url = url;
            Price = price;
        }

    }
}
