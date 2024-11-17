import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { User } from '../../models/user';
import { ApiService } from '../../services/api.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [HeaderComponent],
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
  
}
