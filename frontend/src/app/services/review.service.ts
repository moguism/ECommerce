import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Review } from '../models/review';
import { NewReview } from '../models/newReview';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  constructor(private api: ApiService) { }

  async addReview(newReview: NewReview): Promise<void>{
    // hacer get todos los productos
    // y  añadirlos al html
    const reviewPath = "Review";

    const reviews = await this.api.post(reviewPath, {newReview});
  }
}
