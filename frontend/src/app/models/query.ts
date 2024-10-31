import { OrdinationDirection } from "./enums/ordination-direction";
import { OrdinationType } from "./enums/ordination-type";
import { ProductType } from "./enums/product-type";

export interface Query {
    ordination_direction : OrdinationDirection
    ordination_type: OrdinationType
    product_type: ProductType

}
