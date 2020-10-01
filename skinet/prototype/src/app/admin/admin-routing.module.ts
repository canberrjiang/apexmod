import { NgModule } from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AdminComponent} from './admin.component';
import { EditProductComponent } from './edit-product/edit-product.component';
import { OrderListComponent } from "./order-list/order-list.component";
import { OrderDetailAdminComponent } from "./order-detail-admin/order-detail-admin.component";

const routes: Routes = [
  {path: '', component: AdminComponent},
  {path: 'create', component: EditProductComponent, data: {breadcrumb: 'Create'}},
  {path: 'edit/:id', component: EditProductComponent, data: {breadcrumb: 'Edit'}},
  {path: 'adminorders', component: OrderListComponent, data: {breadcrumb: 'Order-list'}},
  {path: 'adminorders/:id', component: OrderDetailAdminComponent, data: {breadcrumb: 'Order-Detail'}},
  {path: 'deliveryinfo', component: OrderListComponent, data: {breadcrumb: 'Order-list'}},
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class AdminRoutingModule { }