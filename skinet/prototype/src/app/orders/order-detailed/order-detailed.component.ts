import { Component, OnInit } from '@angular/core';
import { IOrder } from 'src/app/shared/models/order';
import { ActivatedRoute, Router, NavigationExtras } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import { OrdersService } from '../orders.service';

@Component({
  selector: 'app-orders-detailed',
  templateUrl: './order-detailed.component.html',
  styleUrls: ['./order-detailed.component.scss'],
})
export class OrderDetailedComponent implements OnInit {
  order: IOrder;
  needPay: boolean;

  constructor(
    private route: ActivatedRoute,
    private breadcrumbService: BreadcrumbService,
    private ordersService: OrdersService,
    private router: Router
  ) {
    this.breadcrumbService.set('@OrderDetailed', '');
  }

  ngOnInit() {
    this.ordersService
      .getOrderDetailed(+this.route.snapshot.paramMap.get('id'))
      .subscribe(
        (order: IOrder) => {
          this.order = order;
          if (order.status !== 'Payment Received') {
            this.needPay = true;
          } else {
            this.needPay = false;
          }
          this.breadcrumbService.set(
            '@OrderDetailed',
            `Order# ${order.id} - ${order.status}`
          );
        },
        (error) => {
          // console.log(error);
        }
      );
  }

  linkToPayPage() {
    const navigationExtras: NavigationExtras = { state: this.order };
    this.router.navigate(['checkout/pay'], navigationExtras);
  }

  print(){
    window.print()
  }
}
