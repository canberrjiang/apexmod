import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckoutPayOrderTotalsComponent } from './checkout-pay-order-totals.component';

describe('CheckoutPayOrderTotalsComponent', () => {
  let component: CheckoutPayOrderTotalsComponent;
  let fixture: ComponentFixture<CheckoutPayOrderTotalsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CheckoutPayOrderTotalsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckoutPayOrderTotalsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
