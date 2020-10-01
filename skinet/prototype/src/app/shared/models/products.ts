export interface IProduct {
  id: number;
  name: string;
  description: string;
  price: number;
  pictureUrl: string;
  // productType: string;
  // productGraphic: string;
  // productBrand: string;
  
  // tags:[]
  photos: IPhoto[];
  childProducts:IComponent[];
  productCategory:string;
  quantity: number;
  tags: any;
  isPublished: boolean;
  information?:string;
  discriminator:string;


}

export interface IProductToCreate {
  name: string;
  description: string;
  price: number;
  pictureUrl: string;
  productCategoryId: number;
  productTagIds :number[];
  selectedChildProducts : IChildProduct[];
  isPublished: boolean;
  discriminator:string;
  information:string;
  // productTypeId: number;
  // productGraphicId: number;
  // productBrand: string;
  // productPlatformId: number;
}

export class ProductFormValues implements IProductToCreate {
  name = '';
  description = '';
  price = 0;
  pictureUrl = '';
  productCategoryId:number;
  productTagIds :number[]=[];
  // childProductIds:IChildProduct[];
  selectedChildProducts:IChildProduct[];
  isPublished = false;
  childProducts=[];
  productCategory ='';
  tags ='';
  discriminator:string;
  information = '';
  // productBrandId: number;
  // productTypeId: number;
  // productPlatformId: number;
  // productGraphicId: number;

  constructor(init?: ProductFormValues) {
    Object.assign(this, init);
  }
  
}

export interface IPhoto {
  id: number;
  pictureUrl: string;
  fileName: string;
  isMain: boolean;
}

export interface IChildProduct {
  childProductId: number,
  IsDefault: boolean
}


export interface IComponent {
  id: number;
  // title: string;
  description: string;
  price: number;
  pictureUrl: string;
  name: string,
  category: string,
  isPublished: true,
  default: boolean
}

export interface IComponentPhoto {
  id: number;
  pictureUrl: string;
  fileName: string;
}

export interface IComponentToCreate {
  title: string;
  description: string;
  pPrice: number;
  tPrice: number;
  productId: number;
}

export class ComponentFormValues implements IComponentToCreate {
  title = '';
  description = '';
  pPrice = 0;
  tPrice = 0;
  productId = null;

  constructor(init?: ComponentFormValues) {
    Object.assign(this, init);
  }
  
}


export interface IChildrenComponent {
  category: string;
  itemsList:[]

}
