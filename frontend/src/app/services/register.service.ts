import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../models/user';
import { catchError, lastValueFrom, Observable } from 'rxjs';
import { Result } from '../models/result';


@Injectable({
  providedIn: 'root'
})
export class RegisterService {

  private BASE_URL = environment.apiUrl;


  constructor(private http: HttpClient) { }


  async addUser<T = void>(path: string, body: Object = {}, contentType = null): Promise<Result<T>> {
    const url = `${this.BASE_URL}${path}`;
    const request$ = this.http.post(url, body, {
      headers: this.getHeader(contentType),
      observe: 'response'
    });

    return this.sendRequest<T>(request$);
  }



  private async sendRequest<T = void>(request$: Observable<HttpResponse<any>>): Promise<Result<T>> {
    let result: Result<T>;
    
    try {
      const response = await lastValueFrom(request$);
      const statusCode = response.status;

      if (response.ok) {
        const data = response.body as T;

        if (data == undefined) {
          result = Result.success(statusCode);
        } else {
          result = Result.success(statusCode, data);
        }
      } else {
        result = result = Result.error(statusCode, response.statusText);
      }
    } catch (exception : any) {
      if (exception instanceof HttpErrorResponse) {
        result = Result.error(exception.status, exception.statusText);
      } else {
        result = Result.error(-1, exception.mesage);
      }
    }

    return result;
  }

  private getHeader(accept = null, contentType = null): HttpHeaders {
    let header: any = {};

    // Para cuando haya que poner un JWT
    //header['Authorization'] = `Bearer ${this.jwt}`;

    if (accept)
      header['Accept'] = accept;

    if (contentType)
      header['Content-Type'] = contentType;

    return new HttpHeaders(header);
  }



}
