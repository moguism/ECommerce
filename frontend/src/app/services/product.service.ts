import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Result } from '../models/result';
import { QuerySelector } from '../models/query-selector';
import { PagedProducts } from '../models/paged-products';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(private api : ApiService) { }
  
  async getAllProducts(querySelector : QuerySelector): Promise<Result<PagedProducts>> 
  {
    return this.api.get<PagedProducts>("Product", {
      "ProductType" : querySelector.productType, 
      "OrdinationType" : querySelector.ordinationType, 
      "OrdinationDirection" : querySelector.ordinationDirection,
      "ProductPageSize" : querySelector.productPageSize,
      "ActualPage" : querySelector.actualPage,
      "Search" : querySelector.search
    }, 'json');
  }

}
