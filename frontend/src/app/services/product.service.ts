import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { QuerySelector } from '../models/query-selector';
import { PagedProducts } from '../models/paged-products';
import { Product } from '../models/product';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(private api: ApiService) { }

  

}
