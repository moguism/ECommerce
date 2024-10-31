import { Injectable } from '@angular/core';
import { Product } from '../models/product';
import { ApiService } from './api.service';
import { Result } from '../models/result';

@Injectable({
  providedIn: 'root'
})
export class ProductService {


  constructor(private api : ApiService) { }


  async getAllProducts(): Promise<Result<Product[]>> {


    return this.api.get<Product[]>("Product",{}, 'json');
    

  }

  async getAllVegetables(): Promise<Result<Product[]>> {


    return this.api.get<Product[]>("Product/vegetables",null,null);
    

  }

  async getAllFruits(): Promise<Result<Product[]>> {


    return this.api.get<Product[]>("Product/fruits",null,null);
    

  }

  async getAllMeats(): Promise<Result<Product[]>> {


    return this.api.get<Product[]>("Product/meat",null,null);
    

  }

}
