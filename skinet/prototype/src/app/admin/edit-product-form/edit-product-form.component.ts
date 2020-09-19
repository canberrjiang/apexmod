import { Component, Input, OnInit, AfterViewInit } from '@angular/core';
import { IProduct, ProductFormValues } from '../../shared/models/products';
// import {IBrand} from '../../shared/models/brand';
// import {IType} from '../../shared/models/productType';
// import { IPlatform } from 'src/app/shared/models/platform';
// import { IGraphic } from 'src/app/shared/models/productGraphic';
import { ICategory } from 'src/app/shared/models/category';
import { ITag } from 'src/app/shared/models/tag';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService } from '../admin.service';

// declare var tinymce: any;
@Component({
  selector: 'app-edit-product-form',
  templateUrl: './edit-product-form.component.html',
  styleUrls: ['./edit-product-form.component.scss'],
})
export class EditProductFormComponent implements OnInit, AfterViewInit {
  @Input() product: ProductFormValues;
  @Input() edit: boolean;
  // @Input() brands: IBrand[];
  // @Input() types: IType[];
  // @Input() platforms: IPlatform[];
  @Input() categories: ICategory[];
  @Input() tags: ITag[];

  success = false;
  editorConfig;

  //for data table
  @Input() rows = [];
  // rows = [];
  selected = [];
  loadingIndicator = true;
  reorderable = true;
  childProductsGroupByCate = [];
  pickChildProduct;
  childProductNewFormatGroup=[]
  pickCategoryId: number;
  columns = [
    { prop: 'id' },
    { name: 'name' },
    { name: 'productCategory' },
    { name: 'price' },
    { name: 'isDefault', sortable: false },
  ];



  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private router: Router
  ) { 

  }

  setRowsData(){
    this.rows = this.product.childProducts;
    this.updateChildProductUpdateFormat()
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
    this.updateChildProductUpdateFormat()
    console.log(this.rows);
  }

  setDefault(id) {
    const resultIndex = this.rows.findIndex((item, index, items) => {
      return item.id === id;
    });
    this.rows[resultIndex].isDefault = !this.rows[resultIndex].isDefault;
    let rows = [...this.rows];
    this.rows = rows;
    this.updateChildProductUpdateFormat();
  }

  RenderChildProduct() {
    console.log(this.pickCategoryId);
    this.adminService.getChildProductByCategory(this.pickCategoryId).subscribe((response: []) => {
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

  ngOnInit(): void {
    // console.log(this.product, this.product.productCategory);
    // console.log(this.categories);
    let self = this;
    this.editorConfig = {
      base_url: '/tinymce', // Root for resources
      suffix: '.min',
      height: 500,
      image_dimensions: false,
      plugins: [
        'image imagetools paste media',
        'advlist autolink lists link charmap print preview anchor',
        'searchreplace visualblocks code fullscreen',
        'insertdatetime media table paste code help wordcount',
      ],
      toolbar: ' undo redo | formatselect | bold italic | image|',
      images_upload_handler: function (blobInfo, success, failure) {
        var formData;
        formData = new FormData();

        formData.append('Photo', blobInfo.blob(), blobInfo.filename());

        const file = blobInfo.blob();
        self.adminService.uploadRichImages(formData).subscribe((response) => {
          console.log(response);
          let url = 'http://104.210.85.29/Content/' + response;
          success(url);
        });
      },
    };
    this.setRowsData()
  }

  ngAfterViewInit() {}

  handleChangeTagIds(id) {
    //console.log(id);
    const checkCurrentTagId = this.product.productTagIds.includes(id);
    // console.log(this.product.productTagIds);
    if (checkCurrentTagId === false) {
      this.product.productTagIds.push(id);
      console.log(this.product);
    } else {
      const index = this.product.productTagIds.findIndex( indexId => indexId === id);
      this.product.productTagIds.splice(index, 1);
    }
    console.log(this.product.productTagIds);
  }
 
  deleteSomeObjectKey() {
    delete this.product.childProducts;
    delete this.product.productCategory;
    delete this.product.tags;
  }

  async onSubmit(product: ProductFormValues) {
    this.deleteSomeObjectKey();
    await this.updateChildProductUpdateFormat();
    if (this.route.snapshot.url[0].path === 'edit') {
      console.log('submit: ', this.product);
      const updatedProduct = {
        ...this.product,
        ...product,
        price: +product.price,
        // productTagIds: [1, 2],
        selectedChildProducts: this.childProductNewFormatGroup,
      };
      console.log('update', updatedProduct);
      this.adminService
        .updateProduct(updatedProduct, +this.route.snapshot.paramMap.get('id'))
        .subscribe((response: any) => {
          this.router.navigate([
            `/admin/edit/${+this.route.snapshot.paramMap.get('id')}`,
          ]);
          // alert(`${response.name} updated!`);
          console.log(response);
          this.success = true;
        });
    } else {
      const newProduct = {
        ...product,
        price: +product.price,
        productTagIds: this.product.productTagIds,
        description:this.product.description
      };

      console.log(0, product);
      console.log(1, newProduct);
      this.adminService.createProduct(newProduct).subscribe((response: any) => {
        this.router.navigate([`/admin/edit/${response}`]);
        // alert(`${response.name} created!`);
        // this.success = true;
      });
    }
  }

  updatePrice(event: any) {
    this.product.price = event;
    //console.log(this.product)
  }

  closeAlert() {
    this.success = false;
  }
}
