import { Product } from "./product";
import { Wishlist } from "./wishlist";

export class ProductsToBuy {
    id: number;
    productId: number;
    quantity: number;
    product: Product;
   
    wishlistId: number;
    wishlist: Wishlist;

    constructor(Id: number, ProductId: number, Quantity: number, Product: Product, WishlistId: number, Wishlist: Wishlist){
        this.id = Id;
        this.productId = ProductId;
        this.quantity = Quantity;
        this.product = Product;
        this.wishlistId = WishlistId;
        this.wishlist = Wishlist;
    }
}
