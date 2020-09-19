import {v4 as uuidv4 } from 'uuid';

export interface IBasket {
    id: string;
    items: IBasketItem[];
    clientSecret?: string;
    paymentIntentId?: string;
    deliveryMethodId?: number;
    shippingPrice?: number;
}

export interface IBasketQuantity {
    quantity: number;
}

export interface IBasketItem {
    id: number;
    price: number;
    quantity: number;
    productName:string;
    pictureUrl: string;
    // brand: string;
    // type: string;
    // platform: string;
    // graphic: string;
    childProducts?:[]
    basketProducts?:[]
    productCategory:string;


}

export class Basket implements IBasket {
    id = uuidv4();
    items: IBasketItem[] = [];
}

export interface IBasketTotals {
    shipping: number;
    subtotal: number;
    total: number;
}

