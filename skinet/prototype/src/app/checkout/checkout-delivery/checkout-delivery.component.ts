import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { CheckoutService } from '../checkout.service';
import { IDeliveryMethod } from 'src/app/shared/models/deliveryMethod';
import { BasketService } from 'src/app/basket/basket.service';

@Component({
  selector: 'app-checkout-delivery',
  templateUrl: './checkout-delivery.component.html',
  styleUrls: ['./checkout-delivery.component.scss'],
})
export class CheckoutDeliveryComponent implements OnInit {
  @Input() checkoutForm: FormGroup;
  deliveryMethods: IDeliveryMethod[];
  @Input() subtotal: number;

  constructor(
    private checkoutService: CheckoutService,
    private basketService: BasketService
  ) {}

  ngOnInit() {
    this.checkoutService.getDeliveryMethods().subscribe(
      (dm: IDeliveryMethod[]) => {
        // this.deliveryMethods = dm;
        if (this.subtotal < 1000) {
          this.deliveryMethods = dm.slice(0, 1);
        } else {
          this.deliveryMethods = dm.slice(1);
        }
      },
      (error) => {
        // console.log(error);
      }
    );
  }

  setShippingPrice(deliveryMethod: IDeliveryMethod) {
    this.basketService.setShippingPrice(deliveryMethod);
  }
}
