import { Component, HostListener, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApiService } from './services/api.service';
import { ShoppingCartService } from './services/shopping-cart.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {
  title = 'frontend';

  constructor(private shoppingCartService: ShoppingCartService) {}

  @HostListener('window:beforeunload', ['$event'])
  async handleBeforeUnload(event: BeforeUnloadEvent): Promise<void> {
    await this.shoppingCartService.saveShoppingCart();
  }


  ngOnInit(): void {
    console.log(window.ethereum);
  }

}

declare global {
  interface Window {
  ethereum: any;
  }
}
  
