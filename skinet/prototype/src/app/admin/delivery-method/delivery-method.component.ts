import { Component, OnInit } from '@angular/core';
import { IDeliveryMethod,IDeliveryMethodToUpdate, DeliveryMethodFormValues } from "../../shared/models/deliveryMethod";
import { AdminService } from '../admin.service';


@Component({
  selector: 'app-delivery-method',
  templateUrl: './delivery-method.component.html',
  styleUrls: ['./delivery-method.component.scss']
})
export class DeliveryMethodComponent implements OnInit {
  deliveryMethod: IDeliveryMethodToUpdate;
  deliveryFormValues: DeliveryMethodFormValues;
  success = false;

  constructor(
    private adminService: AdminService
  ) { 
    this.deliveryMethod = new DeliveryMethodFormValues()
  }

  ngOnInit(): void {
    this.loadDeliveryMethod()
  }

  loadDeliveryMethod(){
    this.adminService.getDeliveryMethod4().subscribe((response:any)=>{
      this.deliveryMethod = {...response};
      // console.log(this.deliveryMethod)
    })
  }


  onSubmit(deliveryMethod: IDeliveryMethodToUpdate) {
      // console.log(deliveryMethod);
      // const updatedProduct = {...this.product, ...product, price: +product.price};
      this.adminService.updateDeliveryMethod4(deliveryMethod).subscribe((response: any) => {
        // console.log('success!')
        this.success = true;
      });
  }

  closeAlert() {
    this.success = false;
  }



  
}
