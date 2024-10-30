import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SearchBarComponent } from '../components/search-bar/search-bar.component';
import { Product } from '../models/product';
import { Observable, forkJoin, lastValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  readonly BASE_URL = 'https://localhost:7150/api/Product/' //URL desde donde se van a recoger todos los productos


  constructor(private http: HttpClient) { }


  //Funcion para recoger todos los productos
  async getAllProducts(): Promise<Product[]> {

    const requests: Observable<Object>[] = [];


    requests.push(this.http.get(`${this.BASE_URL}`)); //almacena en el array anterior todos los productos

    /*
     * Espera a que se terminen de hacer todas la peticiones a la URL
    *  y almacena en el nuevo array todos los objetos ordenados 
    */
    const allDataRaw: any[] = await lastValueFrom(forkJoin(requests));

    const products: Product[] = [];

    for (const data of allDataRaw) {

      const product: Product = {

        id: data.id,
        name: data.name,
        description: data.description,
        price: data.price,
        stock: data.stock,
        average: data.average,
        categoryId: data.categoryId,
        image_url: data.image_url

      };
      //Lo inserta en el array de los productos
      products.push(product);

    }

    //Retorna el array con todos los productos obtenidos
    return products;


  }

  //Funcion para recoger todas las verduras
  async getAllVegetables(): Promise<Product[]> {

    const requests: Observable<Object>[] = [];


    requests.push(this.http.get(`${this.BASE_URL}vegetables`)); 


    const allDataRaw: any[] = await lastValueFrom(forkJoin(requests));

    const products: Product[] = [];

    for (const data of allDataRaw) {

      const product: Product = {

        id: data.id,
        name: data.name,
        description: data.description,
        price: data.price,
        stock: data.stock,
        average: data.average,
        categoryId: data.categoryId,
        image_url: data.image_url

      };
      products.push(product);

    }

    return products;


  }

  //Funcion para recoger todas las frutas
  async getAllFruits(): Promise<Product[]> {

    const requests: Observable<Object>[] = [];


    requests.push(this.http.get(`${this.BASE_URL}fruits`)); 


    const allDataRaw: any[] = await lastValueFrom(forkJoin(requests));

    const products: Product[] = [];

    for (const data of allDataRaw) {

      const product: Product = {

        id: data.id,
        name: data.name,
        description: data.description,
        price: data.price,
        stock: data.stock,
        average: data.average,
        categoryId: data.categoryId,
        image_url: data.image_url

      };
      products.push(product);

    }

    return products;


  }

  //Funcion para recoger todas las carnes
  async getAllMeats(): Promise<Product[]> {

    const requests: Observable<Object>[] = [];


    requests.push(this.http.get(`${this.BASE_URL}meat`)); 


    const allDataRaw: any[] = await lastValueFrom(forkJoin(requests));

    const products: Product[] = [];

    for (const data of allDataRaw) {

      const product: Product = {

        id: data.id,
        name: data.name,
        description: data.description,
        price: data.price,
        stock: data.stock,
        average: data.average,
        categoryId: data.categoryId,
        image_url: data.image_url

      };
      products.push(product);

    }

    return products;


  }


}
