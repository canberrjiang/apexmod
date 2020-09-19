import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { IProduct } from '../shared/models/products';
import { ShopService } from './shop.service';
// import { IBrand } from '../shared/models/brand';
// import { IType } from '../shared/models/productType';
import { IPlatform } from '../shared/models/platform';
import { IGraphic } from '../shared/models/productGraphic';
import { ITag } from '../shared/models/tag';
import { ShopParams } from '../shared/models/shopParams';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  @ViewChild('search') searchTerm: ElementRef;
  products: IProduct[];
  // brands: IBrand[];
  // platforms: IPlatform[];
  // graphics: IGraphic[];
  tags: ITag[];
  shopParams: ShopParams;
  totalCount: number;
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low to High', value: 'priceAsc' },
    { name: 'Price: High to Low', value: 'priceDesc' }
  ];

  constructor(private shopService: ShopService) {
    this.shopParams = this.shopService.getShopParams();
  }

  ngOnInit() {
    this.getProducts(true);
    // this.getBrands();
    // this.getTypes();
    // this.getPlatforms();
    // this.getGraphics();
    this.getTags();
  }

  getProducts(useCache = false) {
    this.shopService.getProducts(useCache).subscribe(response => {
      this.products = response.data;
      this.totalCount = response.count;
    }, error => {
      console.log(error);
    });
  }

  getTags() {
    this.shopService.getTags().subscribe(response => {
      this.tags = [{ id: 0, name: 'All' }, ...response];
    }, error => {
      console.log(error);
    });
  }


  // getTypes() {
  //   this.shopService.getTypes().subscribe(response => {
  //     this.types = [{ id: 0, name: 'All' }, ...response];
  //   }, error => {
  //     console.log(error);
  //   });
  // }

  // getPlatforms() {
  //   this.shopService.getPlatforms().subscribe(response => {
  //     this.platforms = [{ id: 0, name: 'All' }, ...response];
  //   }, error => {
  //     console.log(error);
  //   });
  // }

  // getGraphics() {
  //   this.shopService.getGraphics().subscribe(response => {
  //     this.graphics = [{ id: 0, name: 'All' }, ...response];
  //   }, error => {
  //     console.log(error);
  //   });
  // }


  // onBrandSelected(brandId: number) {
  //   const params = this.shopService.getShopParams();
  //   params.brandId = brandId;
  //   params.pageNumber = 1;
  //   this.shopService.setShopParams(params);
  //   this.getProducts();
  // }

  // onPlatformSelected(platformId: number) {
  //   const params = this.shopService.getShopParams();
  //   params.platformId = platformId;
  //   params.pageNumber = 1;
  //   this.shopService.setShopParams(params);
  //   this.getProducts();
  // }



  // onGraphicSelected(graphicId: number) {
  //   const params = this.shopService.getShopParams();
  //   params.graphicId = graphicId;
  //   params.pageNumber = 1;
  //   this.shopService.setShopParams(params);
  //   this.getProducts();
  // }

  // onTypeSelected(typeId: number) {
  //   const params = this.shopService.getShopParams();
  //   params.typeId = typeId;
  //   params.pageNumber = 1;
  //   this.shopService.setShopParams(params);
  //   this.getProducts();
  // }
    onTagSelected(tagId: number) {
    const params = this.shopService.getShopParams();
    params.producttagid = tagId;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.getProducts();
  }

  onSortSelected(sort: string) {
    const params = this.shopService.getShopParams();
    params.sort = sort;
    this.shopService.setShopParams(params);
    this.getProducts();
  }

  onPageChanged(event: any) {
    const params = this.shopService.getShopParams();
    if (params.pageNumber !== event) {
      params.pageNumber = event;
      this.shopService.setShopParams(params);
      this.getProducts(true);
    }
  }

  onSearch() {
    const params = this.shopService.getShopParams();
    params.search = this.searchTerm.nativeElement.value;
    params.pageNumber = 1;
    this.shopService.setShopParams(params);
    this.getProducts();
  }

  onReset() {
    this.searchTerm.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.shopService.setShopParams(this.shopParams);
    this.getProducts();
  }
}
