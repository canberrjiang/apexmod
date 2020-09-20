import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CheckoutComponent } from './checkout.component';
import { CheckoutRoutingModule } from './checkout-routing.module';
import { SharedModule } from '../shared/shared.module';
import { CheckoutAddressComponent } from './checkout-address/checkout-address.component';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';

import { CheckoutSuccessComponent } from './checkout-success/checkout-success.component';

import { CheckoutPayComponent } from './checkout-pay/checkout-pay.component';


@NgModule({
  declarations: [CheckoutComponent, CheckoutAddressComponent, CheckoutDeliveryComponent, CheckoutReviewComponent, CheckoutSuccessComponent, CheckoutPayComponent],
  imports: [
    CommonModule,
    CheckoutRoutingModule,
    SharedModule
  ]
    
})
export class CheckoutModule { }
