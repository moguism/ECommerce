import { OrdinationDirection } from "./enums/ordination-direction";
import { OrdinationType } from "./enums/ordination-type";
import { ProductType } from "./enums/product-type";

export class QuerySelector {
    productType: ProductType;
    ordinationType: OrdinationType;
    ordinationDirection : OrdinationDirection;
    productPageSize : number;
    actualPage : number;
    search: string;



    public constructor(productType : ProductType, ordinationType : OrdinationType, ordinationDirection : OrdinationDirection,
        productPageSize : number, actualPage : number, search: string )
    {

        this.productType = productType;
        this.ordinationType = ordinationType;
        this.ordinationDirection = ordinationDirection;
        this.productPageSize = productPageSize;
        this.actualPage = actualPage;
        this.search = search;
    }
}
