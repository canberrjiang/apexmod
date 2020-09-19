import { Component, OnInit } from '@angular/core';
import { IBasket, IBasketQuantity } from 'src/app/shared/models/basket';
import { BasketService } from 'src/app/basket/basket.service';
import { Observable } from 'rxjs';
import { IUser } from 'src/app/shared/models/user';
import { AccountService } from 'src/app/account/account.service';
import { async } from 'rxjs/internal/scheduler/async';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {
  basket$: Observable<IBasket>;
  currentUser$: Observable<IUser>
  isAdmin$: Observable<boolean>;
  quantity$: Observable<IBasketQuantity>;
  isCollapsed = true;

  constructor(private basketService: BasketService, private accountService: AccountService) { }

  ngOnInit() {
    this.basket$ = this.basketService.basket$;
    this.currentUser$ = this.accountService.currentUser$;
    this.isAdmin$ = this.accountService.isAdmin$;
    this.quantity$ = this.basketService.quantityNum$;
  }

  logout() {
    this.accountService.logout();
  }


}
