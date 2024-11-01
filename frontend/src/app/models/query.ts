import { OrdinationDirection } from "./enums/ordination-direction"
import { OrdinationType } from "./enums/ordination-type"
import { ProductType } from "./enums/product-type"

export class Query {
    productType: ProductType;
    ordinationType: OrdinationType;
    ordinationDirection : OrdinationDirection;
    productPageName : number;
    productPageSize : number;
    actualPage : number;



    public constructor(productType : ProductType, ordinationType : OrdinationType, ordinationDirection : OrdinationDirection,  
        productPageName : number , productPageSize : number, actualPage : number )
    {

        this.productType = productType;
        this.ordinationType = ordinationType;
        this.ordinationDirection = ordinationDirection;
        this.productPageName = productPageName;
        this.productPageSize = productPageSize;
        this.actualPage = actualPage;
    }
}
