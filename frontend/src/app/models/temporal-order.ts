import { CartContent } from "./cart-content";

export class TemporalOrder {
    cartContentDtos: CartContent[];
    quick: boolean;

    constructor(cartContent: CartContent[], quick: boolean)
    {
        this.cartContentDtos = cartContent,
        this.quick = quick
    }
}
