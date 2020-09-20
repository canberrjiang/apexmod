import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { FormGroup } from '@angular/forms';
import { IBasket } from 'src/app/shared/models/basket';
import { BasketService } from 'src/app/basket/basket.service';
import { CdkStepper } from '@angular/cdk/stepper';
import { ToastrService } from 'ngx-toastr';
import { CheckoutService } from '../checkout.service';
import { Router, NavigationExtras } from '@angular/router';

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss'],
})
export class CheckoutReviewComponent implements OnInit {
  @Input() checkoutForm: FormGroup;
  @Input() appStepper: CdkStepper;
  basket$: Observable<IBasket>;

  constructor(
    private basketService: BasketService,
    private checkoutService: CheckoutService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit() {
    this.basket$ = this.basketService.basket$;
  }

  createPaymentIntent() {
    return this.basketService.createPaymentIntent().subscribe(
      (response: any) => {
        // this.toastr.success('success and created')
        this.appStepper.next();
      },
      (error) => {
        console.log(error);
        this.toastr.error(error.massage);
      }
    );
  }

  async createOrderIntent() {
    const basket = this.basketService.getCurrentBasketValue();
    // await this.createOrder(basket).then(
    //   (response: any) => {
    //     // this.toastr.success('success and created')
    //     console.log('xxx', response);
    //     const orderId = { orderId: response.id };
    //     this.checkoutForm.get('OrderForm').reset(orderId);
    //     // console.log(this.checkoutForm);
    //     this.appStepper.next();
    //   },
    //   (error) => {
    //     console.log(error);
    //     this.toastr.error(error.massage);
    //   }
    // );
    const createdOrder = await this.createOrder(basket);
    console.log(createdOrder);
    this.basketService.deleteBasket(basket);
    const navigationExtras: NavigationExtras = { state: createdOrder };
    this.router.navigate(['checkout/pay'], navigationExtras);


  }

  private async createOrder(basket: IBasket) {
    const orderToCreate = this.getOrderToCreate(basket);
    return this.checkoutService.creatOrder(orderToCreate).toPromise();
  }

  private getOrderToCreate(basket: IBasket) {
    return {
      basketId: basket.id,
      deliveryMethodId: +this.checkoutForm
        .get('deliveryForm')
        .get('deliveryMethod').value,
      shipToAddress: this.checkoutForm.get('addressForm').value,
    };
  }
}
