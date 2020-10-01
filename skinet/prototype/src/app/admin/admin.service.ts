import { Injectable } from '@angular/core';
import {environment} from '../../environments/environment';
import {ProductFormValues, ComponentFormValues} from '../shared/models/products';
import { IDeliveryMethodToUpdate } from "../shared/models/deliveryMethod";
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



  // getAllChildProduct(){
  //   return this.http.get(this.baseUrl+ 'products/discriminator/childproduct');
  // }

  getChildProductByCategory(id){
    return this.http.get(this.baseUrl+ 'products/productcategory/' +id);
  }

  uploadRichImages(formData) {
    return this.http.post(this.baseUrl + 'products/richtextphoto', formData, {
      responseType: 'text'
    });
  }


  deleteComponentPhoto(componentId: number,photoId: number) {
    return this.http.delete(this.baseUrl + 'productcomponent/' + componentId + '/componentphoto/' + photoId);
  }

  getOrdersForAdmin(){
    return this.http.get(this.baseUrl + 'orders/all');
  }

  getOrderDetailedByAdmin(id: number) {
    return this.http.get(this.baseUrl + 'orders/all/' + id);
  }

  getDeliveryMethod4(){
    return this.http.get(this.baseUrl + 'orders/deliverymethods/4')
  }

  updateDeliveryMethod4(deliveryMethod:IDeliveryMethodToUpdate){
    return this.http.put(this.baseUrl + 'orders/deliverymethods/4', deliveryMethod)
  }


}