import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { ShopService } from '../../shop/shop.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductFormValues, IProduct } from '../../shared/models/products';
import { ICategory } from 'src/app/shared/models/category';
import { forkJoin } from 'rxjs';
import { ITag } from 'src/app/shared/models/tag';
@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  styleUrls: ['./edit-product.component.scss'],
})
export class EditProductComponent implements OnInit {
  product: IProduct;
  productFormValues: ProductFormValues;

  categories: ICategory[];
  tags: ITag[];
  success = false;
  edit = true;
  aChildProducts = [];

  constructor(
    private adminService: AdminService,
    private shopService: ShopService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.productFormValues = new ProductFormValues();
  }

  ngOnInit() {
    const categories = this.getCategories();
    const tags = this.getTags();

    forkJoin([categories, tags]).subscribe(
      (results) => {
        this.categories = results[0];
        this.tags = results[1];
      },
      (error) => {
        // console.log(error);
      },
      () => {
        if (this.route.snapshot.url[0].path === 'edit') {
          this.loadProduct();
        }
      }
    );
    if (this.route.snapshot.url[0].path === 'create') {
      this.edit = false;
    }
  }

  updatePrice(event: any) {
    this.product.price = event;
  }

  loadProduct() {
    this.shopService
      .getProduct(+this.route.snapshot.paramMap.get('id'))
      .subscribe((response: any) => {
        const productCategoryId =
          this.categories &&
          this.categories.find((x) => x.name === response.productCategory).id;
        this.product = response;
        this.productFormValues = { ...response, productCategoryId };
        this.aChildProducts = this.productFormValues.childProducts;
      });
  }

  getCategories() {
    return this.shopService.getCategories();
  }
  getTags() {
    return this.shopService.getTags();
  }
}
