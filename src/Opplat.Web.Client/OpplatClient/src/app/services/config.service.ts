import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Configuration {
  baseUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  public static configuration: Configuration;
  constructor(private httpClient: HttpClient) { }

  Load(): void {
    this.httpClient.get<Configuration>('/assets/opplat.config.json').subscribe(r => {
      ConfigService.configuration = r;
    });
  }

  GetConfiguration(): Observable<Configuration> {
    return this.httpClient.get<Configuration>('/assets/opplat.config.json');
  }
}
