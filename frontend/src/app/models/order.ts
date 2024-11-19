import { Product } from "./product";

export class Order {
    Id: number;
    CreatedAt: Date;
    PaymentTypeId: number;
    UserId : number;
    Products: Product[];

    constructor(id: number, createdAt: Date, PaymentTypeId : number , UserId: number, products: Product[])
    {
        this.Id = id,
        this.CreatedAt = createdAt,
        this.PaymentTypeId = PaymentTypeId,
        this.UserId = UserId,
        this.Products = products
    }
}
