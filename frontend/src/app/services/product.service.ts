import { Injectable } from '@angular/core';
import { Product } from '../models/product';
import { ApiService } from './api.service';
import { Result } from '../models/result';

@Injectable({
  providedIn: 'root'
})
export class ProductService {


  constructor(private api : ApiService) { }


  async getProductByName(name: string): Promise<Result<string[]>>
  {
    return this.api.get<string[]>(`smartSearch?query=${name}`)
  }
  
  async getAllProducts(category: number): Promise<Result<Product[]>> 
  {
    return this.api.get<Product[]>("Product", {"ProductType" : category, "OrdinationType" : 0, "OrdinationDirection" : 0}, 'json');
  }

}
