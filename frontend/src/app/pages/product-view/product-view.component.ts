import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { SearchBarComponent } from '../../components/search-bar/search-bar.component';


@Component({
  selector: 'app-product-view',
  standalone: true,
  imports: [HeaderComponent, SearchBarComponent],
  templateUrl: './product-view.component.html',
  styleUrl: './product-view.component.css'
})
export class ProductViewComponent {

  constructor() {
    
  }

}
