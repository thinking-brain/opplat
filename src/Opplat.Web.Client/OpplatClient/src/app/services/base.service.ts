import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { ConfigService, Configuration } from './config.service';

export interface ServiceResponse{
  status: boolean;
  message: string;
  errors: string[];
}

@Injectable({
  providedIn: 'root'
})
export class BaseService<T> {
  config : Configuration;
  baseUrl: string = '';
  selectedEntity: T | null = null;
  constructor(protected httpClient: HttpClient){
    this.config = ConfigService.configuration;
    this.baseUrl = this.config.baseUrl;
  }

  private handleError(error: HttpErrorResponse) {
    let message = ''
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      message = 'Error de conexion con el servidor.';
      console.error('An error occurred:', error.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      message = 'El servicio ha retornado una respuesta insatisfactoria.';
      console.error(
        `Backend returned code ${error.status}, body was: `, error.error);
    }
    // Return an observable with a user-facing error message.
    return of({
      status: false,
      message: message,
      errors: [error.error]
    } as ServiceResponse);
  }

  get(id: string | number): Observable<T>{
    return this.httpClient.get<T>(this.baseUrl + id);
  }

  list(): Observable<T[]>{
    return this.httpClient.get<T[]>(this.baseUrl);
  }

  post(entity: T): Observable<ServiceResponse>{
    return this.httpClient.post<ServiceResponse>(this.baseUrl, entity).pipe(
      catchError(this.handleError)
    );
  }

  put(entity: T): Observable<ServiceResponse>{
    return this.httpClient.put<ServiceResponse>(this.baseUrl, entity).pipe(
      catchError(this.handleError)
    );
  }

  delete(id: string | number): Observable<ServiceResponse>{
    return this.httpClient.delete<ServiceResponse>(this.baseUrl + id).pipe(
      catchError(this.handleError)
    );
  }

}
