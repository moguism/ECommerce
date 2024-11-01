import { Injectable } from '@angular/core';
import { Product } from '../models/product';
import { ApiService } from './api.service';
import { Result } from '../models/result';
import { QuerySelector } from '../models/query-selector';

@Injectable({
  providedIn: 'root'
})
export class ProductService {


  constructor(private api : ApiService) { }


  async getProductByName(name: string): Promise<Result<string[]>>
  {
    return this.api.get<string[]>(`smartSearch?query=${name}`)
  }
  
  async getAllProducts(querySelector : QuerySelector): Promise<Result<Product[]>> 
  {

    

    return this.api.get<Product[]>("Product", {
      "ProductType" : querySelector.productType, 
      "OrdinationType" : querySelector.ordinationType, 
      "OrdinationDirection" : querySelector.ordinationDirection,
      "ProductPageName" : querySelector.productPageName,
      "ProductPageSize" : querySelector.productPageSize,
      "ActualPage" : querySelector.actualPage
    }, 'json');
  }

}
