export interface IDeliveryMethod {
  shortName: string;
  deliveryTime: string;
  description: string;
  price: number;
  id: number;
}

export interface IDeliveryMethodToUpdate {
  shortName: string;
  deliveryTime: string;
  description: string;
  price: number;
}

export class DeliveryMethodFormValues implements IDeliveryMethodToUpdate {
  shortName = '';
  deliveryTime = '';
  description = '';
  price = 50;
}
