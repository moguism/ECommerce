import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';

// Pipe Import
import { CorrectDatePipe } from '../../pipes/correct-date.pipe';
import { Product } from '../../models/product';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [HeaderComponent, CorrectDatePipe, EurosToCentsPipe],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
}) 
export class UserComponent {
  constructor(private userService: UserService){
    this.getUser()
    
  }

  user: User | null = null;

  async getUser(){

    const result = await this.userService.getUser();
    if(result){
      this.user = result;
    }
  }

  totalprice(products: Product[]) {
    let totalcount = 0;
    for (const product of products) {
      totalcount += product.total * product.price;
    }
    return totalcount;
  }
  
}
