import { Component, OnInit } from '@angular/core';
import { IOrder } from 'src/app/shared/models/order';
import { Router } from '@angular/router';

@Component({
  selector: 'app-checkout-error',
  templateUrl: './checkout-error.component.html',
  styleUrls: ['./checkout-error.component.scss'],
})
export class CheckoutErrorComponent implements OnInit {
  order: IOrder;

  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    const state = navigation && navigation.extras && navigation.extras.state;
    if (state) {
      this.order = state as IOrder;
    }
  }
  ngOnInit() {}
}
