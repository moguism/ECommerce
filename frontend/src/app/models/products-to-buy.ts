import { Product } from "./product";
import { Wishlist } from "./wishlist";

export class ProductsToBuy {
    id: number;
    productId: number;
    quantity: number;
    product: Product;
    purchasePrice: number;
    wishlistId: number;
    wishlist: Wishlist;

    constructor(Id: number, ProductId: number, Quantity: number, Product: Product, 
        purchasePrice: number, WishlistId: number, Wishlist: Wishlist){
        this.id = Id;
        this.productId = ProductId;
        this.quantity = Quantity;
        this.product = Product;
        this.purchasePrice = purchasePrice;
        this.wishlistId = WishlistId;
        this.wishlist = Wishlist;
    }
}
