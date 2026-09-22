import { Component, input } from '@angular/core';
import { ProductResponse } from '../../models/product.model';

@Component({
  imports: [],
  selector: 'app-products',
  styleUrl: './products.css',
  templateUrl: './products.html',
})
export class Products {
  products = input.required<ProductResponse[]>();
}
