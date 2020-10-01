import { Component, OnInit, Input } from '@angular/core';
import { IProduct } from '../../models/products';
import { BasketService } from '../../../basket/basket.service';

@Component({
  selector: 'app-product-items',
  templateUrl: './product-items.component.html',
  styleUrls: ['./product-items.component.scss']
})
export class ProductItemsComponent implements OnInit {
  @Input() product: IProduct;

  constructor(private basketService: BasketService) { }

  ngOnInit() {
  }

  addItemToBasket() {
    this.basketService.addItemToBasket(this.product, 1);
  }

}