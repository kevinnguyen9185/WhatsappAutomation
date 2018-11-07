import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpwraperService {
  constructor(private http: HttpClient) {}

  createAuthorizationHeader(headers: HttpHeaders) {
    headers.append('tk', localStorage.getItem('logintoken'));
  }

  get<T>(url):Observable<T> {
    let headers = new HttpHeaders();
    this.createAuthorizationHeader(headers);
    return this.http.get<T>(url, {
      headers: headers
    });
  }

  post<T>(url, data):Observable<T> {
    let headers = new HttpHeaders();
    this.createAuthorizationHeader(headers);
    return this.http.post<T>(url, data, {
      headers: headers
    });
  }
}
