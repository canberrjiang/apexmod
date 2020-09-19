import { Component, Input, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';

@Component({
  selector: 'app-producttable',
  templateUrl: './producttable.component.html',
  styleUrls: ['./producttable.component.scss'],
})
export class ProducttableComponent implements OnInit {
  rows = [];
  selected = [];
  loadingIndicator = true;
  reorderable = true;
  childProductsGroupByCate = [];
  pickChildProduct;
  childProductNewFormatGroup=[]
  pickCategoryId: number;

  @Input() categories: any;
  

  columns = [
    { prop: 'id' },
    { name: 'name' },
    { name: 'productCategory' },
    { name: 'price' },
    { name: 'isDefault', sortable: false },
  ];

  // ColumnMode = ColumnMode;

  constructor(private adminService: AdminService) {
    this.fetch((data) => {
      this.rows = data;
      setTimeout(() => {
        this.loadingIndicator = false;
      }, 1500);
    });
  }

  fetch(cb) {
    const req = new XMLHttpRequest();
    req.open('GET', `assets/data/company.json`);

    req.onload = () => {
      cb(JSON.parse(req.response));
    };

    req.send();
  }

  deleteProduct(id) {
    console.log(id);

    const resultIndex = this.rows.findIndex((item, index, items) => {
      return item.id === id;
    });

    if (resultIndex === -1) {
      return this.rows;
    }

    let rows = [...this.rows];
    rows.splice(resultIndex, 1);
    this.rows = rows;

    console.log(this.rows);
  }

  setDefault(id) {
    const resultIndex = this.rows.findIndex((item, index, items) => {
      return item.id === id;
    });
    this.rows[resultIndex].isDefault = !this.rows[resultIndex].isDefault;
    let rows = [...this.rows];
    this.rows = rows;
  }

  RenderChildProduct() {
    console.log(this.pickCategoryId);
    this.adminService.getAllChildProduct().subscribe((response: []) => {
      // console.log(response);
      this.childProductsGroupByCate = response;
      console.log(this.childProductsGroupByCate);
    });
  }

  HandleChildProduct() {
    console.log(this.pickChildProduct);
  }


  handleAddChildProduct() {
    let newChildProduct = this.changeChildProductData(this.pickChildProduct);
    // console.log('row',this.rows)
    // console.log('row',newChildProduct)
    let rows = [...this.rows, newChildProduct];
    this.rows = rows;
    this.updateChildProductUpdateFormat()
  }
  // getAllChildProduct(){
  //   return this.http.get(this.baseUrl+ '/products/discriminator/childproduct');
  // }

  changeChildProductData(product) {
    return {
      id: product.id,
      name: product.name,
      description: product.description,
      productCategory: product.productCategory,
      price: product.price,
      pictureUrl: product.pictureUrl,
      isPublished: product.isPublished,
      isDefault: false,
    };
  }

  updateChildProductUpdateFormat(){
    const newRows = this.rows.map((element, idx, elements)=>{
      const newElement = {childProductId: element.id,  isDefault: element.isDefault};
      return newElement;
    })
    this.childProductNewFormatGroup = newRows;
    console.log( this.childProductNewFormatGroup);
  }

  // mapChildProductToNewFormat(product) {
  //   return {
  //       childProductId: product.id,
  //       isDefault: product.isDefault
  //     };
  // }

  onActivate(event) {
    console.log('Activate Event', event);
  }

  ngOnInit(): void {}
}
