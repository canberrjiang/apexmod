import { Component, OnInit } from '@angular/core';
import { IProduct, IChildrenComponent } from 'src/app/shared/models/products';
import { ShopService } from '../shop.service';
import { ActivatedRoute } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import { BasketService } from 'src/app/basket/basket.service';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryImageSize,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class ProductDetailsComponent implements OnInit {
  product: IProduct;
  quantity = 1;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  components: IChildrenComponent[];
  childComponentsId: any;
  childComponentsPrice: any;
  childComponentsName: any;
  childProducts: any;
  basketProduct: any;

  constructor(
    private shopService: ShopService,
    private activateRoute: ActivatedRoute,
    private bcService: BreadcrumbService,
    private basketService: BasketService
  ) {
    this.bcService.set('@productDetails', '');
  }

  ngOnInit() {
    this.loadProduct();
  }

  initializeGallery() {
    this.galleryOptions = [
      {
        width: '500px',
        height: '600px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Fade,
        imageSize: NgxGalleryImageSize.Contain,
        thumbnailSize: NgxGalleryImageSize.Contain,
        preview: false,
      },
    ];
    this.galleryImages = this.getImages();
  }

  setPrice(data) {
    let count = 0;
    for (let key in data) {
      count += data[key];
    }
    this.product.price = count;
    return count;
  }

  handleChange(productCategory, id, price,name) {
    this.childComponentsId[productCategory] = id;
    this.childComponentsPrice[productCategory] = price;
    this.childComponentsName[productCategory] = name;
    console.log(this.childComponentsId);
    console.log(this.childComponentsPrice);
    console.log(this.childComponentsName);
    this.setPrice(this.childComponentsPrice);
  }

  mapChildrenProductsId(arr) {
    let components = {};
    arr.forEach((items, index) => {
      let products = components[items.productCategory] || [];
      if (items.isDefault) {
        products = items.id;
      }
      components[items.productCategory] = products;
    });
    return components;
  }

  mapChildrenProductsPrice(arr) {
    let priceGroup = {};
    arr.forEach((items, index) => {
      let products = priceGroup[items.productCategory] || [];
      if (items.isDefault) {
        products = items.price;
      }
      priceGroup[items.productCategory] = products;
    });
    this.setPrice(priceGroup);
    return priceGroup;
  }

  mapChildrenProductsName(arr) {
    let priceGroup = {};
    arr.forEach((items, index) => {
      let products = priceGroup[items.productCategory] || [];
      if (items.isDefault) {
        products = items.name;
      }
      priceGroup[items.productCategory] = products;
    });
    return priceGroup;
  }

  mapChildrenProductsForRender(arr) {
    let components = [];
    arr.forEach((items, i) => {
      let index = -1;
      let alreadyExists = components.some((newItem, j) => {
        if (items.productCategory === newItem.category) {
          index = j;
          return true;
        }
      });
      if (!alreadyExists) {
        components.push({
          category: items.productCategory,
          itemsList: [items],
        });
      } else {
        components[index].itemsList.push(items);
      }
    });
    return components;
  }

  getImages() {
    const imageUrls = [];
    for (const photo of this.product.photos) {
      imageUrls.push({
        small: photo.pictureUrl,
        medium: photo.pictureUrl,
        big: photo.pictureUrl,
      });
    }
    return imageUrls;
  }

  handlerChangeChildrenProductsObjectToArry() {
    let input = this.childComponentsId;
    let output = [];
    for (var type in input) {
      let item = {};
      item[type] = input[type];
      output.push(item);
    }
    this.childProducts = output;
  }
  handlerChangeProductNameObjectToArry() {
    let input = this.childComponentsName;
    let output = [];
    for (var type in input) {
      let item = {};
      item[type] = input[type];
      output.push(item);
    }
    this.basketProduct = output;
  }

  addItemToBasket() {
    // console.log(this.product);
    this.handlerChangeChildrenProductsObjectToArry();
    this.handlerChangeProductNameObjectToArry();
    console.log(this.basketProduct);

    this.basketService.addItemToBasket(
      this.product,
      this.quantity,
      this.childProducts,
      this.basketProduct
    );
  }

  incrementQuantity() {
    this.quantity++;
  }

  decrementQuantity() {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  loadProduct() {
    this.shopService
      .getProduct(+this.activateRoute.snapshot.paramMap.get('id'))
      .subscribe(
        (product) => {
          this.product = product;
          console.log(this.product);
          this.bcService.set('@productDetails', product.name);
          this.initializeGallery();

          if (this.product.productCategory==="pc") {
            const componentGroup = this.mapChildrenProductsForRender(
              this.product.childProducts
            );
            const idGroup = this.mapChildrenProductsId(
              this.product.childProducts
            );
            const priceGroup = this.mapChildrenProductsPrice(
              this.product.childProducts
            );
            const nameGroup = this.mapChildrenProductsName(
              this.product.childProducts
            );


            this.components = componentGroup;
            this.childComponentsId = idGroup;
            this.childComponentsPrice = priceGroup;
            this.childComponentsName = nameGroup;
            console.log('array', this.components);
            console.log('id', this.childComponentsId);
            console.log('price', this.childComponentsPrice);
            console.log('Name', this.childComponentsName);

          }


          // this.product.price = this.setPrice(this.componentTotalPrice)
          // console.log(this.setPrice(this.componentTotalPrice));
        },
        (error) => {
          console.log(error);
        }
      );
  }
}
