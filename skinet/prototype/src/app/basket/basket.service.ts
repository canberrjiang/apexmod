import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import { IBasket, IBasketItem, Basket, IBasketTotals, IBasketQuantity } from '../shared/models/basket';
import { map } from 'rxjs/operators';
import { IProduct } from '../shared/models/products';
import { IDeliveryMethod } from '../shared/models/deliveryMethod';


@Injectable({
  providedIn: 'root'
})
export class BasketService {
  baseUrl = environment.apiUrl;
  private basketSource = new BehaviorSubject<IBasket>(null);
  basket$ = this.basketSource.asObservable();
  private basketTotalSource = new BehaviorSubject<IBasketTotals>(null);
  basketTotal$ = this.basketTotalSource.asObservable();
  shipping = 0;
  private quantityNumSource = new BehaviorSubject<IBasketQuantity>(null);
  quantityNum$ = this.quantityNumSource.asObservable();


  constructor(private http: HttpClient) { }

  createPaymentIntent() {
    // console.log(this.getCurrentBasketValue());
    return this.http.post(this.baseUrl + 'payments/' + this.getCurrentBasketValue().id, {})
      .pipe(
        map((basket: IBasket) => {
          this.basketSource.next(basket);
        })
      );
  }

  

  setShippingPrice(deliveryMethod: IDeliveryMethod) {
    // console.log(deliveryMethod.id)
    if(deliveryMethod.id === 4){
      this.shipping = deliveryMethod.price;
      const basket = this.getCurrentBasketValue();
      basket.deliveryMethodId = deliveryMethod.id;
      basket.shippingPrice = deliveryMethod.price;
      // this.calculateTotalWithDeposit();
      this.calculateTotals();
      this.setBasket(basket);
    }else{
      this.shipping = deliveryMethod.price;
      const basket = this.getCurrentBasketValue();
      basket.deliveryMethodId = deliveryMethod.id;
      basket.shippingPrice = deliveryMethod.price;
      this.calculateTotals();
      this.setBasket(basket);
    }



  }

  getBasket(id: string) {
    return this.http.get(this.baseUrl + 'basket?id=' + id)
      .pipe(
        map((basket: IBasket) => {
          this.basketSource.next(basket);
          this.shipping = basket.shippingPrice;
         this.calculateTotals();
         this.calculateItemsQuantity();
        })
      );
  }

  setBasket(basket: IBasket) {
    return this.http.post(this.baseUrl + 'basket', basket).subscribe((response: IBasket) => {
      this.basketSource.next(response);
      
      this.calculateTotals();
      this.calculateItemsQuantity();
    }, error => {
      console.log(error);
    });
  }

  getCurrentBasketValue() {
    return this.basketSource.value;
  }

  addItemToBasket(item: IProduct, quantity = 1, childProducts?:[],basketProduct?:[]) {
    const itemToAdd: IBasketItem = this.mapProductItemToBasketItem(item, quantity, childProducts,basketProduct);
    let basket = this.getCurrentBasketValue();
    if (basket === null) {
      basket = this.createBasket();
    }
    console.log(basket)
    console.log("itemToAdd",itemToAdd)
   basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity);
   this.setBasket(basket);
  }
  //increase quantity in basket
  incrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    basket.items[foundItemIndex].quantity++;
    this.setBasket(basket);
  }

  decrementItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    if (basket.items[foundItemIndex].quantity > 1) {
      basket.items[foundItemIndex].quantity--;
      this.setBasket(basket);
    } else {
      this.removeItemFromBasket(item);
    }
  }

  removeItemFromBasket(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    //check is the item in the basket, if true
    if (basket.items.some(x => x.id === item.id)) {
      //if true, filter and take out, make other items to a new array 
      basket.items = basket.items.filter(i => i.id !== item.id);
      
      if (basket.items.length > 0) {
        this.setBasket(basket);
      } else {
        this.deleteBasket(basket);
      }
    }
  }

  deleteLocalBasket(id: string) {
    this.basketSource.next(null);
    this.basketTotalSource.next(null);
    localStorage.removeItem('basket_id');
  }

  deleteBasket(basket: IBasket) {
    return this.http.delete(this.baseUrl + 'basket?id=' + basket.id).subscribe(() => {
      this.basketSource.next(null);
      this.basketTotalSource.next(null);
      this.quantityNumSource.next(null);
      localStorage.removeItem('basket_id');
    }, error => {
      console.log(error);
    });
  }

  private calculateTotals() {
    const basket = this.getCurrentBasketValue();
    const shipping = this.shipping;
    const subtotal = basket.items.reduce((prevValue, item) => (item.price * item.quantity) + prevValue, 0);
    let total = 0;
    if(shipping == 20){
      total = shipping ;
    }else{
      total = subtotal + shipping;
    }
    
    this.basketTotalSource.next({shipping, total, subtotal});
  }

  // private calculateTotalWithDeposit() {
  //   const basket = this.getCurrentBasketValue();
  //   const shipping = this.shipping;
  //   const subtotal = basket.items.reduce((prevValue, item) => (item.price * item.quantity) + prevValue, 0);
  //   const total = 50;
  //   console.log(shipping, total, subtotal)
  //   this.basketTotalSource.next({shipping, total, subtotal});
  // }

  private calculateItemsQuantity(){
    const basket = this.getCurrentBasketValue();
    const quantity =  basket.items.reduce((prevValue, item) =>  item.quantity + prevValue, 0);
    this.quantityNumSource.next({quantity})
  }





  private addOrUpdateItem(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
    const index = items.findIndex(i => i.id === itemToAdd.id);
    if (index === -1) {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    } else if(itemToAdd.productCategory === "pc"){


      itemToAdd.quantity = quantity + items[index].quantity;
      items[index] = itemToAdd;
    }
    else {
      items[index].quantity += quantity;
    }
    return items;
  }

  private createBasket(): IBasket {
    const basket = new Basket();
    localStorage.setItem('basket_id', basket.id);
    return basket;
  }

  private mapProductItemToBasketItem(item: IProduct, quantity: number,childProducts?:[],basketProduct?:[]): IBasketItem {
    return {
      id: item.id,
      productName: item.name,
      price: item.price,
      pictureUrl: item.pictureUrl,
      quantity,
      childProducts: childProducts,
      productCategory: item.productCategory,
      basketProducts:basketProduct
      // brand: item.productBrand,
      // type: item.productType
      // platform: item.productPlatform,
      // graphic: item.productGraphic,
    };
  }
}
