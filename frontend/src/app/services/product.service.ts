import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  readonly BASE_URL = 'https://pokeapi.co/api/v2/' //URL desde donde se van a recoger los datos de los pokemoms


  constructor() { }
}
