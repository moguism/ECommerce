import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { lastValueFrom, Observable } from 'rxjs';
import { Result } from '../models/result';


@Injectable({
  providedIn: 'root'
})
export class RegisterService {

  private BASE_URL = environment.apiUrl;
  jwt : string = ""

  constructor(private http: HttpClient) { }

  async registerUser<T = void>(path: string, body: Object = {}): Promise<Result<T>> {
    const url = `${this.BASE_URL}${path}`;
    const request$ = this.http.post(url, body, {
      headers: this.getHeader(null),
      observe: 'response',
      responseType: 'text'
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
      console.log("EXCEPCION: ", exception)
      if (exception instanceof HttpErrorResponse) {
        result = Result.error(exception.status, exception.statusText);
      } else {
        result = Result.error(-1, exception.mesage);
      }
    }

    console.log("RESULT: ", result)
    if(result.data)
    {
      this.jwt = result.data.toString();
      console.log("HAY MI MADRE EL BICHO: ", this.jwt)
    }
    return result;
  }

  private getHeader(accept = null, contentType = "application/json"): HttpHeaders {
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
