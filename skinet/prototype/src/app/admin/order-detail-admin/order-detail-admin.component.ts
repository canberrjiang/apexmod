import { Component, OnInit } from '@angular/core';
import { IOrder } from 'src/app/shared/models/order';
import { ActivatedRoute, Router,NavigationExtras } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import { AdminService } from '../admin.service';

@Component({
  selector: 'app-order-detail-admin',
  templateUrl: './order-detail-admin.component.html',
  styleUrls: ['./order-detail-admin.component.scss']
})
export class OrderDetailAdminComponent implements OnInit {

  order: IOrder;

  constructor(
    private route: ActivatedRoute,
    private breadcrumbService: BreadcrumbService,
    private adminService: AdminService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.adminService
    .getOrderDetailedByAdmin(+this.route.snapshot.paramMap.get('id'))
    .subscribe(
      (order: IOrder) => {
        this.order = order;
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

  print(){
    window.print()
  }

}
