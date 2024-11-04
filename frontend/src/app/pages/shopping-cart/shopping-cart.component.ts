import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [],
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.css'
})
export class ShoppingCartComponent implements OnInit {


  numOfTotalProducts = 0;
  totalPrice = 0.0;
  totalProducts : Product[] = []
  numOfIndividualProduct = 0




  constructor(){}


  async ngOnInit(): Promise<void> {

  }



}
