import { Component, OnInit, Input } from '@angular/core';
import { IProduct } from 'src/app/shared/models/products';
import { AdminService } from '../admin.service';
import {ToastrService} from 'ngx-toastr';


@Component({
  selector: 'app-edit-product-component',
  templateUrl: './edit-product-component.component.html',
  styleUrls: ['./edit-product-component.component.scss']
})
export class EditProductComponentComponent implements OnInit {

  @Input() product: IProduct;

  constructor(private adminService: AdminService,toast: ToastrService) { }

  ngOnInit(): void {
  }

  deleteProductComponent(productcomponentId: number) {
    this.adminService.deleteProductComponent(productcomponentId, this.product.id).subscribe(() => {
      //const photoIndex = this.product.photos.findIndex(x => x.id === productcomponentId);
      //this.product.photos.splice(photoIndex, 1);

      
      // this.product.productComponents.splice(this.product.productComponents.findIndex(p => p.id === productcomponentId), 1);
    }, error => {
      //this.toast.error('Problem deleting photo');
      console.log(error);
    });
  }

  showComponent( component : any ){
    //console.log(component)
  }

}
