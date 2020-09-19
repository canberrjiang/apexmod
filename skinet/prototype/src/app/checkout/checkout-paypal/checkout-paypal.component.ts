import {
  Component,
  OnInit,
  Input,
  ViewChild,
  ElementRef,
  AfterViewInit,
} from '@angular/core';
import { BasketService } from 'src/app/basket/basket.service';
import { CheckoutService } from '../checkout.service';
import { ToastrService } from 'ngx-toastr';
import { IBasket } from 'src/app/shared/models/basket';
import { IOrder } from 'src/app/shared/models/order';
import { Router, NavigationExtras } from '@angular/router';
import * as dropin from 'braintree-web-drop-in';

declare var paypal;

@Component({
  selector: 'app-checkout-paypal',
  templateUrl: './checkout-paypal.component.html',
  styleUrls: ['./checkout-paypal.component.scss'],
})
export class CheckoutPaypalComponent implements OnInit, AfterViewInit {
  loading = false;
  @Input() totalPay: number;
  @ViewChild('paypal', { static: true }) paypalElement: ElementRef;

  // product = {
  //   price: this.totalPay,
  //   description:"test-product",
  //   img:'assets/1.png'
  // }

  paidFor = false;

  constructor(
    private basketService: BasketService,
    private checkoutService: CheckoutService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  braintreeIsReady: boolean;
  dropIninstance: any;

  ngOnInit() {
    // paypal
    // .Buttons({
    //   createOrder:(data, actions) => {
    //     return actions.order.create({
    //       purchase_units: [
    //         {
    //           description:"test-product",
    //           amount: {
    //             currency_code: 'AUD',
    //             value: this.totalPay
    //           }
    //         }
    //       ]
    //     })
    //   },
    //   onApprove: async (data, actions) => {
    //     const order = await actions.order.capture();
    //     //this.paidFor = true;
    //     console.log(order);
    //     this.router.navigate(['checkout/success']);
    //   },
    //   onError: err => {
    //     console.log(err);
    //   }
    // })
    // .render(this.paypalElement.nativeElement);
  }

  ngAfterViewInit() {
    // dropin.create(
    //   {
    //     authorization:
    //       'eyJ2ZXJzaW9uIjoyLCJhdXRob3JpemF0aW9uRmluZ2VycHJpbnQiOiJleUowZVhBaU9pSktWMVFpTENKaGJHY2lPaUpGVXpJMU5pSXNJbXRwWkNJNklqSXdNVGd3TkRJMk1UWXRjMkZ1WkdKdmVDSXNJbWx6Y3lJNkltaDBkSEJ6T2k4dllYQnBMbk5oYm1SaWIzZ3VZbkpoYVc1MGNtVmxaMkYwWlhkaGVTNWpiMjBpZlEuZXlKbGVIQWlPakUyTURBMU1UZ3lNVFFzSW1wMGFTSTZJamt3TjJaalpqY3hMV0k1TVdNdE5ESXlNaTFoTURFNExUWXpOMlppTVRkalpETTFNQ0lzSW5OMVlpSTZJbU15ZUhOd2FISTVOWEF5ZGprMmNXNGlMQ0pwYzNNaU9pSm9kSFJ3Y3pvdkwyRndhUzV6WVc1a1ltOTRMbUp5WVdsdWRISmxaV2RoZEdWM1lYa3VZMjl0SWl3aWJXVnlZMmhoYm5RaU9uc2ljSFZpYkdsalgybGtJam9pWXpKNGMzQm9jamsxY0RKMk9UWnhiaUlzSW5abGNtbG1lVjlqWVhKa1gySjVYMlJsWm1GMWJIUWlPbVpoYkhObGZTd2ljbWxuYUhSeklqcGJJbTFoYm1GblpWOTJZWFZzZENKZExDSnpZMjl3WlNJNld5SkNjbUZwYm5SeVpXVTZWbUYxYkhRaVhTd2liM0IwYVc5dWN5STZlMzE5Lk1HS0t3cWtmNWo0VmdEREx5UTRpa05HMWdubzBzUlZXUUh3YlV5NkNQTXl1RU5NSENDRXUzcXlubGIyMXBta2dhSkRZalk5TFFBeV9Ed2xaUTVGTFlRIiwiY29uZmlnVXJsIjoiaHR0cHM6Ly9hcGkuc2FuZGJveC5icmFpbnRyZWVnYXRld2F5LmNvbTo0NDMvbWVyY2hhbnRzL2MyeHNwaHI5NXAydjk2cW4vY2xpZW50X2FwaS92MS9jb25maWd1cmF0aW9uIiwiZ3JhcGhRTCI6eyJ1cmwiOiJodHRwczovL3BheW1lbnRzLnNhbmRib3guYnJhaW50cmVlLWFwaS5jb20vZ3JhcGhxbCIsImRhdGUiOiIyMDE4LTA1LTA4IiwiZmVhdHVyZXMiOlsidG9rZW5pemVfY3JlZGl0X2NhcmRzIl19LCJjbGllbnRBcGlVcmwiOiJodHRwczovL2FwaS5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tOjQ0My9tZXJjaGFudHMvYzJ4c3Bocjk1cDJ2OTZxbi9jbGllbnRfYXBpIiwiZW52aXJvbm1lbnQiOiJzYW5kYm94IiwibWVyY2hhbnRJZCI6ImMyeHNwaHI5NXAydjk2cW4iLCJhc3NldHNVcmwiOiJodHRwczovL2Fzc2V0cy5icmFpbnRyZWVnYXRld2F5LmNvbSIsImF1dGhVcmwiOiJodHRwczovL2F1dGgudmVubW8uc2FuZGJveC5icmFpbnRyZWVnYXRld2F5LmNvbSIsInZlbm1vIjoib2ZmIiwiY2hhbGxlbmdlcyI6W10sInRocmVlRFNlY3VyZUVuYWJsZWQiOnRydWUsImFuYWx5dGljcyI6eyJ1cmwiOiJodHRwczovL29yaWdpbi1hbmFseXRpY3Mtc2FuZC5zYW5kYm94LmJyYWludHJlZS1hcGkuY29tL2MyeHNwaHI5NXAydjk2cW4ifSwicGF5cGFsRW5hYmxlZCI6dHJ1ZSwicGF5cGFsIjp7ImJpbGxpbmdBZ3JlZW1lbnRzRW5hYmxlZCI6dHJ1ZSwiZW52aXJvbm1lbnROb05ldHdvcmsiOmZhbHNlLCJ1bnZldHRlZE1lcmNoYW50IjpmYWxzZSwiYWxsb3dIdHRwIjp0cnVlLCJkaXNwbGF5TmFtZSI6IkNvZGVJc0FydCIsImNsaWVudElkIjoiQVVJMWFtWHplUlFUTG5EXzkzamlxTTRGclBqb2xnQ2hmRlZDZDdRVk1aSF9OTzd6OG1RZFZpdlNVT0dTYVdnTVg2MlJ6cDljdXVyZEZTVmwiLCJwcml2YWN5VXJsIjoiaHR0cDovL2V4YW1wbGUuY29tL3BwIiwidXNlckFncmVlbWVudFVybCI6Imh0dHA6Ly9leGFtcGxlLmNvbS90b3MiLCJiYXNlVXJsIjoiaHR0cHM6Ly9hc3NldHMuYnJhaW50cmVlZ2F0ZXdheS5jb20iLCJhc3NldHNVcmwiOiJodHRwczovL2NoZWNrb3V0LnBheXBhbC5jb20iLCJkaXJlY3RCYXNlVXJsIjpudWxsLCJlbnZpcm9ubWVudCI6Im9mZmxpbmUiLCJicmFpbnRyZWVDbGllbnRJZCI6Im1hc3RlcmNsaWVudDMiLCJtZXJjaGFudEFjY291bnRJZCI6ImNvZGVpc2FydCIsImN1cnJlbmN5SXNvQ29kZSI6IkFVRCJ9fQ==',
    //     selector: '#dropin-container',
    //   },
    //   (err, dropinInstance) => {
    //     if (err) {
    //       // Handle any errors that might've occurred when creating Drop-in
    //       console.error(err);
    //       return;s
    //     }
    //     this.dropIninstance = dropinInstance;
    //     this.braintreeIsReady = true;
    //   }
    // );
  }



  // async submitPayPal() {
  //   console.log(this.totalPay)
  // }
}
