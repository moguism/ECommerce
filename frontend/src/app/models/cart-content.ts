export class CartContent {
    ProductId: number;
    Quantity: number;

    constructor(productId: number, quantity: number)
    {
        this.ProductId = productId;
        this.Quantity = quantity;
    }
}
