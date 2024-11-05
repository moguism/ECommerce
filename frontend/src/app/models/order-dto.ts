import { Product } from "./product";

export class OrderDto
{
    id: number;
    products: Product[];

    constructor(id: number, products: Product[])
    {
        this.id = id,
        this.products = products
    }
}
