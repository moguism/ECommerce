import { ProductsToBuy } from "./products-to-buy";

export class Wishlist {

    id: number;
    products: ProductsToBuy[];

    constructor(Id: number, Products: ProductsToBuy[]){
        this.id = Id;
        this.products = Products;
    }

}
