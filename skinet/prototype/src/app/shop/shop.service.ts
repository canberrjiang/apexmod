import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { IPagination, Pagination } from '../shared/models/pagination';
// import { IBrand } from '../shared/models/brand';
// import { IPlatform } from '../shared/models/platform';

// import { IType } from '../shared/models/productType';
// import { IGraphic } from '../shared/models/productGraphic';

import { ICategory } from '../shared/models/category';
import { ITag } from '../shared/models/tag';
import { map, delay } from 'rxjs/operators';
import { ShopParams } from '../shared/models/shopParams';
import { IProduct } from '../shared/models/products';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  // baseUrl = 'https://localhost:5001/api/';
  baseUrl = 'http://104.210.85.29/api/';

  products: IProduct[] = [];
  // brands: IBrand[] = [];
  // platforms: IPlatform[] = [];

  // types: IType[] = [];
  // graphics: IGraphic[] = [];
  categories: ICategory[] = [];
  tags: ITag[] = [];

  pagination = new Pagination();
  shopParams = new ShopParams();

  constructor(private http: HttpClient) {}

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

    // if (this.shopParams.brandId !== 0) {
    //   params = params.append('brandId', this.shopParams.brandId.toString());
    // }
    // if (this.shopParams.platformId !== 0) {
    //   params = params.append('platformId', this.shopParams.platformId.toString());
    // }

    // if (this.shopParams.typeId !== 0) {
    //   params = params.append('typeId', this.shopParams.typeId.toString());
    // }
    // if (this.shopParams.graphicId !== 0) {
    //   params = params.append('graphicId', this.shopParams.graphicId.toString());
    // }
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

  getShopParams() {
    return this.shopParams;
  }

  setShopParams(params: ShopParams) {
    this.shopParams = params;
  }

  getProduct(id: number) {
    // const product = this.products.find(p => p.id === id);

    // if (product) {
    //   return of(product);
    // }
    return this.http.get<IProduct>(this.baseUrl + 'products/' + id);
  }

  // getBrands() {
  //   if (this.brands.length > 0) {
  //     return of(this.brands);
  //   }
  //   return this.http.get<IBrand[]>(this.baseUrl + 'products/brands').pipe(
  //     map(response => {
  //       this.brands = response;
  //       return response;
  //     })
  //   );
  // }

  // getPlatforms() {
  //   if (this.platforms.length > 0) {
  //     return of(this.platforms);
  //   }
  //   return this.http.get<IPlatform[]>(this.baseUrl + 'products/platforms').pipe(
  //     map(response => {
  //       this.platforms = response;
  //       return response;
  //     })
  //   );
  // }

  // getGraphics() {
  //   if (this.graphics.length > 0) {
  //     return of(this.graphics);
  //   }
  //   return this.http.get<IGraphic[]>(this.baseUrl + 'products/graphics').pipe(
  //     map(response => {
  //       this.graphics = response;
  //       return response;
  //     })
  //   );
  // }

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
