import { Product } from "./product";

export class Order {
    id: number;
    createdAt: Date;
    isReserved: number;
    products: Product[];
    paymentTypeId: number;

    constructor(id: number, createdAt: Date, isReserved: number, products: Product[])
    {
        this.id = id,
        this.createdAt = createdAt,
        this.isReserved = isReserved,
        this.products = products,
        this.paymentTypeId = 0
    }
}
