import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SearchBarComponent } from '../components/search-bar/search-bar.component';
import { Product } from '../models/product';
import { Observable, forkJoin, lastValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  readonly BASE_URL = 'https://localhost:7150/api/Product' //URL desde donde se van a recoger todos los productos


  constructor(private http: HttpClient) { }


  //Funcion para recoger todos los productos
  async getAllProducts(): Promise<Product[]> { /*Recoge de forma asíncrona todos los productos desde la BASE_URL y los recoge en un array de producots */

    const requests: Observable<Object>[] = []; 


    requests.push(this.http.get(`${this.BASE_URL}`)); //almacena en el array anterior todos los productos

    /**Espera a que se terminen de hacer todas la peticiones a la URL
    *Y almacena en el nuevo array todos los objetos de los pokemon ordenados */
    const allDataRaw: any[] = await lastValueFrom(forkJoin(requests));

    //Array donde se van a almacenar todos los objetos tipo Producto
    const products: Product[] = [];

    //Recorre el array de los productos ordenados
    for (const data of allDataRaw) {
      //Almacena en esta variable el pokemon con todos sus datos
      const product: Product = {

        id: data.id,
        name: data.name,
        description : data.description,
        price : data.price,
        stock : data.stock,
        average : data.average,
        categoryId : data.categoryId,
        image_url : data.image_url

      };
      //Lo inserta en el array de los pokemon
      products.push(product);

    }

    //Retorna el array
    return products;


  }



}
