import { Product } from "./product";
import { Wishlist } from "./wishlist";

export class Order {
    id: number;
    createdAt: Date;
    paymentTypeId: number;
    userId : number;
    wishlist: Wishlist;


    constructor(id: number, createdAt: Date, PaymentTypeId : number , UserId: number, Wishlist: Wishlist)
    {
        this.id = id,
        this.createdAt = createdAt,
        this.paymentTypeId = PaymentTypeId,
        this.userId = UserId
        this.wishlist = Wishlist;
    }
}
