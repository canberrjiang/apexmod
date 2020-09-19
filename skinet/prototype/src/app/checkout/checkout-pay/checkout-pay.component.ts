import { Component, OnInit, AfterViewInit } from '@angular/core';
import { IOrder } from 'src/app/shared/models/order';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { IBasketTotals } from 'src/app/shared/models/basket';
import { BasketService } from 'src/app/basket/basket.service';
import * as dropin from 'braintree-web-drop-in';
import { CheckoutService } from '../checkout.service';

@Component({
  selector: 'app-checkout-pay',
  templateUrl: './checkout-pay.component.html',
  styleUrls: ['./checkout-pay.component.scss']
})
export class CheckoutPayComponent implements OnInit, AfterViewInit {
  basketTotals$: Observable<IBasketTotals>;
  order:IOrder;
  // order$:IOrder;
  braintreeIsReady: boolean;
  dropIninstance: any;
  braintreeKey:any;
  
  constructor(private router: Router,private basketService: BasketService,private checkoutService: CheckoutService,) { 
    const navigation = this.router.getCurrentNavigation();
    // this.getPayPalToken()
    const state = navigation && navigation.extras && navigation.extras.state;
    if (state) {
      this.order = state as IOrder; 
      console.log(this.order);
    }

    this.getPayPalToken()
   
  }

  getPayPalToken(){
    this.checkoutService.getPayPalToken().subscribe((response:string) => {
      this.braintreeKey = response;

      dropin.create(
        {
          authorization: this.braintreeKey,
          selector: '#dropin-container',
          // container: '#dropin-container',
          // bank card payments are enabled by default. To enable paypal option, check out the below line.
          paypal: {
            flow: 'vault'
          }
        },
        (err, dropinInstance) => {
          if (err) {
            // Handle any errors that might've occurred when creating Drop-in
            console.error(err);
            return;
          }
          this.dropIninstance = dropinInstance;
          this.braintreeIsReady = true;
        }
      );
    })
  }

  ngOnInit(){
    this.basketTotals$ = this.basketService.basketTotal$;
    //  this.getPayPalToken();

  }

  ngAfterViewInit() {
    // this.getPayPalToken();
    // console.log(this.braintreeKey);
  }



  pay() {
    this.dropIninstance.requestPaymentMethod((err, payload) => {
      if (err) {
        // deal with error
        console.log(err);
      }
      else {
        console.log(payload)

        const paymentData = {
          orderid: this.order && this.order.id,
          nonce: payload.nonce,
        };
        //send nonce to the server
        this.checkoutService.handlePayPalPayment(paymentData).subscribe((response) => {
          console.log(response); 
        })
      }
    });
  }
}
