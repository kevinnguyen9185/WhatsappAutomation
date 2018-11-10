import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpwraperService {
  constructor(private http: HttpClient) {}

  getAuthorizeHeader(){
    var header = new HttpHeaders();
    if(localStorage.getItem('logintoken')){
      header = new HttpHeaders({
        'tk':localStorage.getItem('logintoken')
      })
    };
    return header; 
  }

  get<T>(url):Observable<T> {
    return this.http.get<T>(url, {
      headers: this.getAuthorizeHeader()
    });
  }

  post<T>(url, data):Observable<T> {
    return this.http.post<T>(url, data, {
      headers: this.getAuthorizeHeader()
    });
  }
}
