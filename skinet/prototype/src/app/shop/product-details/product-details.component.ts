import {
  Component,
  OnInit,
  Directive,
  Renderer2,
  HostListener,
  HostBinding,
  ElementRef,
  ViewChild,
} from '@angular/core';
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

// @Directive({
//   selector: "[ccCardHover]"
// })
// class CardHoverDirective {
//   // @HostBinding('class.card-outline-primary')private ishovering: boolean;

//   constructor(private el: ElementRef,
//               private renderer: Renderer2) {
//     // renderer.setElementStyle(el.nativeElement, 'backgroundColor', 'gray');
//   }

//   @HostListener('mouseover') onMouseOver() {
//     let part = this.el.nativeElement.querySelector('.preview-container');
//     this.renderer.setStyle(part, 'display', 'block');
//     // this.ishovering = true;
//   }

//   @HostListener('mouseout') onMouseOut() {
//     let part = this.el.nativeElement.querySelector('.preview-container');
//     this.renderer.setStyle(part, 'display', 'none');
//     // this.ishovering = false;
//   }
// }

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
  childComponentsImg: any;
  childProducts: any;
  basketProduct: any;
  @ViewChild('descriptionComponents') descriptionComponents: ElementRef;
  // @ViewChild('previewContainer')  previewContainer: ElementRef;

  @HostListener('window:scroll') onScroll(e: Event): void {
    // console.log(window.pageYOffset,this.descriptionComponents.nativeElement.getBoundingClientRect().top, window.innerHeight);
    // console.log(this.descriptionComponents.nativeElement.getBoundingClientRect().top);

    let distance = this.descriptionComponents.nativeElement.getBoundingClientRect()
      .top;
    let part = this.el.nativeElement.querySelector('.preview-container');
    if (distance <= 0) {
      this.renderer.setStyle(part, 'display', 'block');
    } else {
      this.renderer.setStyle(part, 'display', 'none');
    }
  }

  constructor(
    private shopService: ShopService,
    private activateRoute: ActivatedRoute,
    private bcService: BreadcrumbService,
    private basketService: BasketService,
    private el: ElementRef,
    private renderer: Renderer2
  ) {
    this.bcService.set('@productDetails', '');

    // document.onscroll = function () {
    //   // var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
    //   // var cHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    //   let scrollTop = document.documentElement.scrollTop;
    //   let cHeight = window.innerHeight || document.documentElement.clientHeight;
    //   let oDiv = document.getElementById('product-component');
    //   if (scrollTop > oDiv.offsetTop - cHeight) alert('触发了');
    // };
  }

  // @HostListener('mouseover') onMouseOver() {
  //   let part = this.el.nativeElement.querySelector('.preview-container');
  //   this.renderer.setStyle(part, 'display', 'block');
  //   // this.ishovering = true;
  // }

  // @HostListener('mouseout') onMouseOut() {
  //   let part = this.el.nativeElement.querySelector('.preview-container');
  //   this.renderer.setStyle(part, 'display', 'none');
  //   // this.ishovering = false;
  // }
  // getYPosition(e: Event): number {
  //   return (e.target as Element).scrollTop;
  // }

  ngOnInit() {
    this.loadProduct();
  }

  initializeGallery() {
    this.galleryImages = this.getImages();
    if (this.galleryImages.length === 1) {
      this.galleryOptions = [
        {
          width: '100%',
          height: '100%',
          imagePercent: 100,
          thumbnailsColumns: 4,
          arrowPrevIcon: 'fa fa-chevron-left',
          arrowNextIcon: 'fa fa-chevron-right',
          imageAnimation: NgxGalleryAnimation.Fade,
          imageSize: NgxGalleryImageSize.Contain,
          thumbnailSize: NgxGalleryImageSize.Contain,
          imageArrows: false,
          preview: false,
        },
      ];
    } else {
      this.galleryOptions = [
        {
          width: '100%',
          height: '100%',
          imagePercent: 100,
          thumbnailsColumns: 4,
          arrowPrevIcon: 'fa fa-chevron-left',
          arrowNextIcon: 'fa fa-chevron-right',
          imageAnimation: NgxGalleryAnimation.Fade,
          imageSize: NgxGalleryImageSize.Contain,
          thumbnailSize: NgxGalleryImageSize.Contain,
          preview: false,
        },
      ];
    }
  }

  setPrice(data) {
    let count = 0;
    for (let key in data) {
      count += data[key];
    }
    this.product.price = count;
    return count;
  }

  handleChange(productCategory, id, price, pictureUrl) {
    this.childComponentsId[productCategory] = id;
    this.childComponentsPrice[productCategory] = price;
    this.childComponentsImg[productCategory] = pictureUrl;
    // console.log(this.childComponentsImg);
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

  mapChildrenProductsImg(arr) {
    let priceGroup = {};
    arr.forEach((items, index) => {
      let products = priceGroup[items.productCategory] || [];
      if (items.isDefault) {
        products = items.pictureUrl;
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
      if (photo.isMain === true) {
        imageUrls.unshift({
          small: photo.pictureUrl,
          medium: photo.pictureUrl,
          big: photo.pictureUrl,
        });
      } else {
        imageUrls.push({
          small: photo.pictureUrl,
          medium: photo.pictureUrl,
          big: photo.pictureUrl,
        });
      }
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
  // handlerChangeProductNameObjectToArry() {
  //   let input = this.childComponentsName;
  //   let output = [];
  //   for (var type in input) {
  //     let item = {};
  //     item[type] = input[type];
  //     output.push(item);
  //   }
  //   this.basketProduct = output;
  // }

  addItemToBasket() {
    this.handlerChangeChildrenProductsObjectToArry();
    // this.handlerChangeProductNameObjectToArry();

    this.basketService.addItemToBasket(
      this.product,
      this.quantity,
      this.childProducts
      // this.basketProduct
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
          this.bcService.set('@productDetails', product.name);
          this.initializeGallery();

          if (this.product.discriminator === 'Product') {
            const componentGroup = this.mapChildrenProductsForRender(
              this.product.childProducts
            );
            const idGroup = this.mapChildrenProductsId(
              this.product.childProducts
            );
            const priceGroup = this.mapChildrenProductsPrice(
              this.product.childProducts
            );
            const imgGroup = this.mapChildrenProductsImg(
              this.product.childProducts
            );

            this.components = componentGroup;
            this.childComponentsId = idGroup;
            this.childComponentsPrice = priceGroup;
            this.childComponentsImg = imgGroup;
            // console.log(this.product);
            // console.log(idGroup,imgGroup);
          }
        },
        (error) => {
          // console.log(error);
        }
      );
  }
}
