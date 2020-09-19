import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule, RoutesRecognized } from "@angular/router";
import {CheckoutComponent} from './checkout.component';
import { CheckoutSuccessComponent } from './checkout-success/checkout-success.component';
import { CheckoutPayComponent } from './checkout-pay/checkout-pay.component';

const routes: Routes = [
  {path:'', component: CheckoutComponent},
  {path:'success', component: CheckoutSuccessComponent},
  {path:'pay', component: CheckoutPayComponent},
]

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class CheckoutRoutingModule { }
