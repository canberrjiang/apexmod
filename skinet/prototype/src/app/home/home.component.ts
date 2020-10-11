import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IProduct } from '../shared/models/products';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  products: IProduct[];
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getPcProduct().subscribe((response:any) => {
      this.products = response.data;
    })
  }

  getPcProduct(){
    return this.http.get(this.baseUrl + 'products?producttagid=3')
  }

}
