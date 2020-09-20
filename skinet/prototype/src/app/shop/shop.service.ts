import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IPagination, Pagination } from '../shared/models/pagination';

import { ICategory } from '../shared/models/category';
import { ITag } from '../shared/models/tag';
import { map, delay } from 'rxjs/operators';
import { ShopParams } from '../shared/models/shopParams';
import { IProduct } from '../shared/models/products';
import { of } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  // baseUrl = 'https://localhost:5001/api/';
  baseUrl = environment.apiUrl;

  //caching
  products: IProduct[] = [];

  categories: ICategory[] = [];
  tags: ITag[] = [];
  pagination = new Pagination();
  shopParams = new ShopParams();

  constructor(private http: HttpClient) { }

  getProducts(useCache: boolean) {
    if (useCache === false) {
      this.products = [];
    }

    if (this.products.length > 0 && useCache === true) {
      const pagesReceived = Math.ceil(
        this.products.length / this.shopParams.pageSize
      );

      if (this.shopParams.pageNumber <= pagesReceived) {
        this.pagination.data = this.products.slice(
          (this.shopParams.pageNumber - 1) * this.shopParams.pageSize,
          this.shopParams.pageNumber * this.shopParams.pageSize
        );

        return of(this.pagination);
      }
    }

    let params = new HttpParams();

    if (this.shopParams.producttagid !== 0) {
      params = params.append(
        'producttagid',
        this.shopParams.producttagid.toString()
      );
    }
    if (this.shopParams.search) {
      params = params.append('search', this.shopParams.search);
    }

    params = params.append('sort', this.shopParams.sort);
    params = params.append('pageIndex', this.shopParams.pageNumber.toString());
    params = params.append('pageSize', this.shopParams.pageSize.toString());

    return this.http
      .get<IPagination>(this.baseUrl + 'products', {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          this.products = [...this.products, ...response.body.data];
          this.pagination = response.body;
          return this.pagination;
        })
      );
  }

  getProductsByAdmin() {
    return this.http.get(this.baseUrl + 'products/admin/products')
  }



  getShopParams() {
    return this.shopParams;
  }

  setShopParams(params: ShopParams) {
    this.shopParams = params;
  }

  getProduct(id: number) {
    //caching, data not match
    // const product = this.products.find(p => p.id === id);
    // if (product) {
    //   return of(product);
    // }
    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }

  getCategories() {
    if (this.categories.length > 0) {
      return of(this.categories);
    }
    return this.http
      .get<ICategory[]>(this.baseUrl + 'products/categories')
      .pipe(
        map((response) => {
          this.categories = response;
          return response;
        })
      );
  }

  getTags() {
    if (this.tags.length > 0) {
      return of(this.tags);
    }
    return this.http.get<ITag[]>(this.baseUrl + 'products/tags').pipe(
      map((response) => {
        this.tags = response;
        return response;
      })
    );
  }
}
