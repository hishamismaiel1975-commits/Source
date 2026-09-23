import { Component, signal, inject } from '@angular/core';
import { Products } from '../../shared/products/products';
import { ProductResponse } from '../../models/product.model';
import { ProductService } from '../../core/services/product.service';
import { Pagination } from '../../models/pagination.model';

@Component({
  imports: [Products],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {
  private readonly productService = inject(ProductService);

  searchText = signal('');
  products = signal<Pagination<ProductResponse>>({
    pageIndex: 0,
    pageSize: 0,
    count: 0,
    totalPages: 0,
    data: [],
  });

  onSearch() {
    if (this.searchText()) {
      this.productService.searchProducts(this.searchText()).subscribe((result) => {
        this.products.set(result.data);
      });
    } else {
      this.productService.getProducts().subscribe((result) => {
        this.products.set(result.data);
      });
    }
  }
}
