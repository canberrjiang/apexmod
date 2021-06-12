import { Component, OnInit, Input } from '@angular/core';
import { IProduct } from 'src/app/shared/models/products';
import { BasketService } from 'src/app/basket/basket.service';

@Component({
  selector: 'app-product-item',
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss'],
})
export class ProductItemComponent implements OnInit {
  @Input() product: IProduct;

  constructor(private basketService: BasketService) { }

  ngOnInit() {
    // this.product.price = this.product.price - this.product.discountPrice;
  }

  addItemToBasket() {
    this.basketService.addItemToBasket(this.product, 1);
  }

  // updatePrice() {
  //   this.product.price = this.product.price - this.product.discountPrice;
  // }
}
