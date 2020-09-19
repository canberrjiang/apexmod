import { NgModule } from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AdminComponent} from './admin.component';
import { EditProductComponent } from './edit-product/edit-product.component';
import { EditComponentComponent } from './edit-component/edit-component.component';
// import {EditProductComponent} from './edit-product/edit-product.component';

const routes: Routes = [
  {path: '', component: AdminComponent},
  {path: 'create', component: EditProductComponent, data: {breadcrumb: 'Create'}},
  {path: 'edit/:id', component: EditProductComponent, data: {breadcrumb: 'Edit'}},
  {path: 'edit/:id/create', component: EditComponentComponent, data: {breadcrumb: 'Create-Component'}},
  {path: 'edit/:id/component/:cid', component: EditComponentComponent, data: {breadcrumb: 'Edit-Component'}}
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class AdminRoutingModule { }