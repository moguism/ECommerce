import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private api: ApiService) { 

  }

  async getUser(): Promise<User | null>{
    const result = await this.api.get("User/authorizedUser", {}, 'json');
    if(result.data){
      const user: User = result.data;
      return user;
    }
    return null;
  }
}
