import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminComponent } from './admin.component';
import { EditProductComponent } from './edit-product/edit-product.component';
import {SharedModule} from '../shared/shared.module';
import {AdminRoutingModule} from './admin-routing.module';
import { EditProductFormComponent } from './edit-product-form/edit-product-form.component';
import { EditProductPhotosComponent } from './edit-product-photos/edit-product-photos.component';
import { TinyEditorComponent } from './tiny-editor/tiny-editor.component';
import { OrderListComponent } from './order-list/order-list.component';
import { OrderDetailAdminComponent } from './order-detail-admin/order-detail-admin.component';
import { DeliveryMethodComponent } from './delivery-method/delivery-method.component';


@NgModule({
  declarations: [AdminComponent, EditProductComponent, EditProductFormComponent, EditProductPhotosComponent,TinyEditorComponent, OrderListComponent, OrderDetailAdminComponent, DeliveryMethodComponent],
  imports: [
    CommonModule,
    SharedModule,
    AdminRoutingModule,
  ]
})
export class AdminModule { }