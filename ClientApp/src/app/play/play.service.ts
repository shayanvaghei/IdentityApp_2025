import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PlayService {
  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {

  }

  getPlayers() {
    return this.http.get(this.apiUrl + 'play/get-players');
  }
}
