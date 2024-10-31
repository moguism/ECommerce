import { Injectable } from '@angular/core';
import { Product } from '../models/product';
import { ApiService } from './api.service';
import { Result } from '../models/result';
import { forkJoin, lastValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {


  constructor(private api : ApiService) { }


  async getProductByName(name: string): Promise<Result<Product[]>>
  {
    return this.api.get<Product[]>("Search", name, 'json')
  }
  
  async getAllProducts(): Promise<Result<Product[]>> {
    return this.api.get<Product[]>("Product",{}, 'json');
  }

  async getAllVegetables(): Promise<Result<Product[]>> {
    return this.api.get<Product[]>("Product/vegetables",null,'json');
  }

  async getAllFruits(): Promise<Result<Product[]>> {
    return this.api.get<Product[]>("Product/fruits",null,'json');
  }

  async getAllMeats(): Promise<Result<Product[]>> {
    return this.api.get<Product[]>("Product/meat",null,'json');
  }

}
