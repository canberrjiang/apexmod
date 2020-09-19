import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { ShopService } from '../../shop/shop.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ComponentFormValues, IProduct, IComponent } from '../../shared/models/products';



@Component({
  selector: 'app-edit-component',
  templateUrl: './edit-component.component.html',
  styleUrls: ['./edit-component.component.scss'],
})
export class EditComponentComponent implements OnInit {
  componentData: IComponent;
  component: ComponentFormValues;
  productId: number;
  componentId: number;
  edit = true;

  constructor(
    private adminService: AdminService,
    private shopService: ShopService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.component = new ComponentFormValues();
  }

  ngOnInit(): void {
    //this.loadProductId();
    if (this.route.snapshot.url[2].path === 'component') {
      this.loadProduct();
    }
   // console.log("id",this.componentId);
   if (this.route.snapshot.url[2].path === 'create') {
    this.edit = false;
    }
  }

  loadProductId() {
    this.productId = +this.route.snapshot.paramMap.get('id');
    this.componentId = +this.route.snapshot.paramMap.get('cid');
  }

  loadProduct() {
    this.shopService.getProduct(this.productId).subscribe((response: any) => {
      //const component = this.component && this.platforms.find(x => x.name === response.productPlatform).id;
      //this.product.productComponents.splice(this.product.productComponents.findIndex(p => p.id === productcomponentId), 1);
      // this.component = response.product;
      // this.productFormValues = {...response};

      const newComponent = response.productComponents.find(
        (x) => x.id === this.componentId
      );
      
      this.component = {...newComponent};
      // this.componentData = {...newComponent}
      console.log("comp",this.component);
    });
  }

  updatePPrice(event: any) {
    this.component.pPrice = event;
  }

  updateTPrice(event: any) {
    this.component.tPrice = event;
  }

  backToProduct(productId: number){
    this.router.navigate([`/admin/edit/${productId}`])
  }

  
}
