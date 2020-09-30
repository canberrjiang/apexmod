import { Component, Input, OnInit, AfterViewInit } from '@angular/core';
import { IProduct, ProductFormValues } from '../../shared/models/products';
import { ICategory } from 'src/app/shared/models/category';
import { ITag } from 'src/app/shared/models/tag';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService } from '../admin.service';


@Component({
  selector: 'app-edit-product-form',
  templateUrl: './edit-product-form.component.html',
  styleUrls: ['./edit-product-form.component.scss'],
})
export class EditProductFormComponent implements OnInit, AfterViewInit {
  @Input() product: ProductFormValues;
  @Input() edit: boolean;

  @Input() categories: ICategory[];
  @Input() tags: ITag[];

  success = false;
  editorConfig;
  editorConfig02;

  //for data table
  @Input() rows = [];

  selected = [];
  loadingIndicator = true;
  reorderable = true;
  childProductsGroupByCate = [];
  pickChildProduct;
  childProductNewFormatGroup = [];
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
  ) {}

  setRowsData() {
    this.rows = this.product.childProducts;
    this.updateChildProductUpdateFormat();
  }

  deleteProduct(id) {

    const resultIndex = this.rows.findIndex((item, index, items) => {
      return item.id === id;
    });

    if (resultIndex === -1) {
      return this.rows;
    }

    let rows = [...this.rows];
    rows.splice(resultIndex, 1);
    this.rows = rows;
    this.updateChildProductUpdateFormat();

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
    // console.log(this.pickCategoryId);
    this.childProductsGroupByCate = [];
    this.pickChildProduct ='none';
    this.adminService
      .getChildProductByCategory(this.pickCategoryId)
      .subscribe((response: []) => {

        this.childProductsGroupByCate = response;
        // console.log(this.childProductsGroupByCate);
      });
  }


  handleAddChildProduct() {
    // console.log(1, this.pickChildProduct); 
    if (this.pickChildProduct === "none" || this.pickChildProduct === undefined ) {
      return
    } else {
          let newChildProduct = this.changeChildProductData(this.pickChildProduct);
    // console.log(2, newChildProduct); 
    let rows = [...this.rows, newChildProduct];
    this.rows = rows;
    this.updateChildProductUpdateFormat();
    }

  }

  HandleChildProduct(value) {
    // console.log(value);
    // console.log(this.pickChildProduct);
    // this.pickChildProduct = 
  }

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

  updateChildProductUpdateFormat() {
    const newRows = this.rows.map((element, idx, elements) => {
      const newElement = {
        childProductId: element.id,
        isDefault: element.isDefault,
      };
      return newElement;
    });
    this.childProductNewFormatGroup = newRows;

  }

  ngOnInit(): void {

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

          let url = 'https://www.apexmod.com.au/Content/' + response;
          success(url);
        });
      },
    };


    this.editorConfig02 = {
      base_url: '/tinymce', // Root for resources
      suffix: '.min',
      height: 500,
      image_dimensions: false,
      plugins: [
        'advlist autolink lists link image charmap print preview anchor',
        'searchreplace visualblocks code fullscreen',
        'insertdatetime media table paste code help wordcount'
      ],
      toolbar:
        'undo redo | formatselect | bold italic backcolor | \
        alignleft aligncenter alignright alignjustify | \
        bullist numlist outdent indent | removeformat |'
    };



    this.setRowsData();
  }

  ngAfterViewInit() {}

  handleChangeTagIds(id) {

    const checkCurrentTagId = this.product.productTagIds.includes(id);

    if (checkCurrentTagId === false) {
      this.product.productTagIds.push(id);
    } else {
      const index = this.product.productTagIds.findIndex(
        (indexId) => indexId === id
      );
      this.product.productTagIds.splice(index, 1);
    }

  }

  deleteSomeObjectKey() {
    delete this.product.childProducts;
    delete this.product.productCategory;
    delete this.product.tags;
  }

  async onSubmit(product: ProductFormValues) {
    this.deleteSomeObjectKey();
    if (this.product.discriminator === 'Product') {
      await this.updateChildProductUpdateFormat();
    }

    if (this.route.snapshot.url[0].path === 'edit') {

      const updatedProduct = {
        ...this.product,
        ...product,
        price: +product.price,

        selectedChildProducts: this.childProductNewFormatGroup,
      };

      this.adminService
        .updateProduct(updatedProduct, +this.route.snapshot.paramMap.get('id'))
        .subscribe((response: any) => {
          this.router.navigate([
            `/admin/edit/${+this.route.snapshot.paramMap.get('id')}`,
          ]);
          this.success = true;
        });
    } else {
      const newProduct = {
        ...product,
        price: +product.price,
        productTagIds: this.product.productTagIds,
        description: this.product.description,
      };
      this.adminService.createProduct(newProduct).subscribe((response: any) => {
        this.router.navigate([`/admin/edit/${response}`]);

      });
    }
  }

  updatePrice(event: any) {
    this.product.price = event;
  }

  closeAlert() {
    this.success = false;
  }
}
