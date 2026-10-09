export interface ProductResponse {
  id: string;
  name: string;
  summary: string;
  description: string;
  imageFile: string;
  productBrandId: string;
  productTypeId: string;
  price: number;
  createdDate: string;
  productBrand: ProductBrandResponse | null;
  productType: ProductTypeResponse | null;
}

export interface ProductBrandResponse {
  id: string;
  name: string;
}

export interface ProductTypeResponse {
  id: string;
  name: string;
}
