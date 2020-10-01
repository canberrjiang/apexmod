import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { IOrder } from 'src/app/shared/models/order';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss']
})
export class OrderListComponent implements OnInit {
  orders: IOrder[];

  constructor(private adminService: AdminService,) { }

  ngOnInit(): void {
    this.getOrders();
  }

  getOrders() {
    this.adminService.getOrdersForAdmin().subscribe((orders: IOrder[]) => {
      this.orders = orders;
    }, error => {
      // console.log(error);
    });
  }

}
