import { Injectable } from '@angular/core';
import {environment} from '../../environments/environment';
import {ProductFormValues, ComponentFormValues} from '../shared/models/products';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createProduct(product: ProductFormValues) {
    return this.http.post(this.baseUrl + 'products', product);
  }

  updateProduct(product: ProductFormValues, id: number) {
    return this.http.put(this.baseUrl + 'products/' + id, product);
  }

  deleteProduct(id: number) {
    return this.http.delete(this.baseUrl + 'products/' + id);
  }

  uploadImage(file: File, id: number) {
    const formData = new FormData();
    formData.append('photo', file, 'image.png');
    return this.http.put(this.baseUrl + 'products/' + id + '/photo', formData, {
      reportProgress: true,
      observe: 'events'
    });
  }


  deleteProductPhoto(photoId: number, productId: number) {
    return this.http.delete(this.baseUrl + 'products/' + productId + '/photo/' + photoId);
  }

  setMainPhoto(photoId: number, productId: number) {
    return this.http.post(this.baseUrl + 'products/' + productId + '/photo/' + photoId, {});
  }

  createComponent(component: ComponentFormValues,productId: number) {
    return this.http.put(this.baseUrl + 'products/'+ productId + '/productcomponent/', component);
  }

  updateComponent(component: ComponentFormValues, id: number) {
    return this.http.put(this.baseUrl + 'productComponent/' + id, component);
  }
  
  deleteProductComponent(productcomponentId: number, productId: number) {
    return this.http.delete(this.baseUrl + 'products/' + productId + '/productcomponent/' + productcomponentId);
  }



  uploadComponentImage(file: File, id: number) {
    const formData = new FormData();
    formData.append('Photo', file, 'image.png');
    return this.http.put(this.baseUrl + 'productcomponent/' + id + '/componentphoto', formData, {
      reportProgress: true,
      observe: 'events'
    });
  }

  getAllChildProduct(){
    return this.http.get(this.baseUrl+ '/products/discriminator/childproduct');
  }

  getChildProductByCategory(id){
    return this.http.get(this.baseUrl+ '/products/productcategory/' +id);
  }


  // uploadRichImage(file: File) {
  //   const formData = new FormData();
  //   formData.append('Photo', file, 'image.png');
  //   return this.http.post(this.baseUrl + 'products/richtextphoto', formData, {
  //     reportProgress: true,
  //     observe: 'events'
  //   });
  // }
  uploadRichImages(formData) {
    return this.http.post(this.baseUrl + 'products/richtextphoto', formData, {
      responseType: 'text'
    });
  }


  deleteComponentPhoto(componentId: number,photoId: number) {
    return this.http.delete(this.baseUrl + 'productcomponent/' + componentId + '/componentphoto/' + photoId);
  }


}