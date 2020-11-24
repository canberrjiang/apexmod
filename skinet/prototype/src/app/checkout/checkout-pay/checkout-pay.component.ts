import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  AfterViewInit,
} from '@angular/core';
import { IOrder } from 'src/app/shared/models/order';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { IBasketTotals } from 'src/app/shared/models/basket';
import { BasketService } from 'src/app/basket/basket.service';
// import * as dropin from 'braintree-web-drop-in';
import { CheckoutService } from '../checkout.service';

declare var paypal;

@Component({
  selector: 'app-checkout-pay',
  templateUrl: './checkout-pay.component.html',
  styleUrls: ['./checkout-pay.component.scss'],
})
export class CheckoutPayComponent implements OnInit, AfterViewInit {
  basketTotals$: Observable<IBasketTotals>;
  order: IOrder;
  // braintreeIsReady: boolean;
  // dropIninstance: any;
  // braintreeKey: any;
  @ViewChild('paypal', { static: true }) paypalElement: ElementRef;
  show = true;

  constructor(
    private router: Router,
    private basketService: BasketService,
    private checkoutService: CheckoutService
  ) {
    const navigation = this.router.getCurrentNavigation();

    const state = navigation && navigation.extras && navigation.extras.state;
    if (state) {
      this.order = state as IOrder;
      this.show = false;
    }

    // this.getPayPalToken();
  }

  // getPayPalToken() {
  //   this.checkoutService.getPayPalToken().subscribe((response: string) => {
  //     this.braintreeKey = response;

  //     dropin.create(
  //       {
  //         authorization: this.braintreeKey,
  //         selector: '#dropin-container',
  //         // container: '#dropin-container',
  //         // bank card payments are enabled by default. To enable paypal option, check out the below line.
  //         paypal: {
  //           flow: 'vault',
  //         },
  //       },
  //       (err, dropinInstance) => {
  //         if (err) {
  //           // Handle any errors that might've occurred when creating Drop-in
  //           console.error(err);
  //           return;
  //         }
  //         this.dropIninstance = dropinInstance;
  //         this.braintreeIsReady = true;
  //       }
  //     );
  //   });
  // }

  ngOnInit() {
    this.basketTotals$ = this.basketService.basketTotal$;
    //  this.getPayPalToken();
    let router = this.router;
    paypal
      .Buttons({
        createOrder: (data, actions) => {
          return fetch(
            `https://www.toplayer.com.au/api/paypal/${this.order.id}`,
            {
              method: 'post',
            }
          )
            .then(function (res) {
              return res.json();
            })
            .then(function (orderData) {
              if (JSON.stringify(orderData) === '{}') {
                router.navigate(['checkout/error']);
              } else {
                return orderData.id;
              }
              // return orderData.id;
            });
        },
        onApprove: (data, actions) => {
          return fetch(
            'https://www.toplayer.com.au/api/paypal/capture/' +
              data.orderID +
              `/${this.order.id}`,
            {
              method: 'post',
            }
          )
            .then(function (res) {
              return res.json();
            })
            .then(function (orderData) {
              var errorDetail =
                Array.isArray(orderData.details) && orderData.details[0];

              if (errorDetail && errorDetail.issue === 'INSTRUMENT_DECLINED') {
                return actions.restart();
              }

              if (errorDetail) {
                let msg = 'Sorry, your transaction could not be processed.';
                if (errorDetail.description)
                  msg += '\n\n' + errorDetail.description;
                if (orderData.debug_id) msg += ' (' + orderData.debug_id + ')';
                // Show a failure message
                return alert(msg);
              }
              // alert(
              //   'Transaction completed by ' + orderData.payer.name.given_name
              // );

              // Show a success message to the buyer
              router.navigate(['checkout/success']);
            });
        },
        // onError: (err) => {
        //   console.log(err);
        // },
      })
      .render(this.paypalElement.nativeElement);
  }

  ngAfterViewInit() {}

  // pay() {
  //   this.dropIninstance.requestPaymentMethod((err, payload) => {
  //     if (err) {
  //       // deal with error
  //       // console.log(err);
  //     } else {

  //       const paymentData = {
  //         orderid: this.order && this.order.id,
  //         nonce: payload.nonce,
  //       };
  //       //send nonce to the server
  //       this.checkoutService
  //         .handlePayPalPayment(paymentData)
  //         .subscribe((response) => {
  //           this.router.navigate(['checkout/success']);
  //         });
  //     }
  //   });
  // }
}
