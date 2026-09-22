import { Component, signal } from '@angular/core';
import { Products } from '../../shared/products/products';
import { ProductResponse } from '../../models/product.model';

@Component({
  imports: [Products],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {
  products = signal<ProductResponse[]>([]);

  onSearch() {
    this.products.set([
      {
        id: '11111111-1111-1111-1111-111111111111',
        name: 'iPhone 17',
        summary: 'Latest Apple smartphone',
        description: 'A powerful smartphone with an advanced camera system.',
        imageFile: '/images/products/iphone-17.jpg',
        productBrandId: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
        productTypeId: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        price: 999.99,
        createdDate: '2026-09-01T10:00:00Z',

        productBrand: {
          id: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
          name: 'Apple',
        },

        productType: {
          id: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
          name: 'Smartphone',
        },
      },

      {
        id: '22222222-2222-2222-2222-222222222222',
        name: 'Galaxy S26',
        summary: 'Samsung flagship smartphone',
        description: 'A premium Android smartphone with a high-resolution display.',
        imageFile: '/images/products/galaxy-s26.jpg',
        productBrandId: 'cccccccc-cccc-cccc-cccc-cccccccccccc',
        productTypeId: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
        price: 899.99,
        createdDate: '2026-09-02T10:00:00Z',

        productBrand: {
          id: 'cccccccc-cccc-cccc-cccc-cccccccccccc',
          name: 'Samsung',
        },

        productType: {
          id: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
          name: 'Smartphone',
        },
      },
    ]);
  }
}
