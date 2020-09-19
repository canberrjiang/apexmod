import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShopComponent } from './shop.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { Routes, RouterModule } from '@angular/router';

export const routes: Routes = [
  {path:'',component: ShopComponent},
  {path:':id',component: ProductDetailsComponent, data: { breadcrumb: {alias: 'productDetails'}} } ,
]

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ],exports :[RouterModule]
})
export class ShopRoutingModule { }
