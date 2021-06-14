import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-checkout-pay-order-totals',
  templateUrl: './checkout-pay-order-totals.component.html',
  styleUrls: ['./checkout-pay-order-totals.component.scss']
})
export class CheckoutPayOrderTotalsComponent implements OnInit {
  @Input() shippingPrice: number;
  @Input() subtotal: number;
  @Input() total: number;
  transaction: number;
  constructor() { }

  ngOnInit(): void {
    this.calculateTransaction();
  }

  calculateTransaction() {
    if (this.shippingPrice === 200) {
      this.transaction = 3;
    } else {
      this.transaction = this.total - this.shippingPrice - this.subtotal;
    }
  }
}
