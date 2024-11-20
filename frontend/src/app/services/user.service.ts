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

  async getAllUsers(): Promise<User[] | null>{
    const result = await this.api.get("User", {}, 'json');
    if(result.data){
      const users = result.data;
      return users;
    }
    return null;
  }

  async deleteUser(id: number): Promise<void>
  {
    await this.api.delete("User", {"id" : id});
  }

  /*async updateUserAdmin(user: User): Promise<User | null>
  {
    const result = await this.api.put<User | null>("User/userAdmin", user)
    return result.data
  }*/

  async updateUser(user: User): Promise<User | null>
  {
    const result = await this.api.put<User | null>("User", user)
    return result.data
  }
}
