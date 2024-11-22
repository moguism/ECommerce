import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { NewReview } from '../models/new-review';
import { Review } from '../models/review';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  constructor(private api: ApiService) { }

  async addReview(newReview: NewReview): Promise<void> {
    // hacer get todos los productos
    // y  añadirlos al html
    const reviewPath = "Review/AddReview";

    try {
      await this.api.post<Review>(reviewPath, newReview); // Enviar directamente el objeto newReview
    } catch (error) {
      console.error("Error al añadir la reseña", error);
    }
  }
}
