import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CoffeeBean } from '../models/coffee-bean.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BeanService {
  private readonly baseUrl = `${environment.apiUrl}/coffeeBeans`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<CoffeeBean[]> {
    return this.http.get<CoffeeBean[]>(this.baseUrl);
  }

  getById(id: number): Observable<CoffeeBean> {
    return this.http.get<CoffeeBean>(`${this.baseUrl}/${id}`);
  }

  getBeanOfTheDay(): Observable<CoffeeBean> {
    return this.http.get<CoffeeBean>(`${this.baseUrl}/bean-of-the-day`);
  }

  search(query: string): Observable<CoffeeBean[]> {
    return this.http.get<CoffeeBean[]>(`${this.baseUrl}/search`, {
      params: { query }
    });
  }

  create(bean: Partial<CoffeeBean>): Observable<CoffeeBean> {
    return this.http.post<CoffeeBean>(this.baseUrl, bean);
  }

  update(id: number, bean: Partial<CoffeeBean>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, bean);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
