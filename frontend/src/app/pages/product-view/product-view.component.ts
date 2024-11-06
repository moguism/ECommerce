import { Component, OnInit } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';


@Component({
  selector: 'app-product-view',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './product-view.component.html',
  styleUrl: './product-view.component.css'
})
export class ProductViewComponent implements OnInit {

  product: Product | null = null
  constructor(private productService: ProductService){}

  async ngOnInit(): Promise<void> {
  }

}
