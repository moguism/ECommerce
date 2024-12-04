import { Product } from "./product";

export class CartContent {
    ProductId: number;
    Quantity: number;
    Product: Product
    constructor(productId: number, quantity: number, product: Product)
    {
        this.ProductId = productId;
        this.Quantity = quantity;
        this.Product = product;
    }
}
