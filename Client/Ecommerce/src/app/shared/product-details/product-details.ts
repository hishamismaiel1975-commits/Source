import { Component, signal, inject, OnInit } from '@angular/core';
import { ProductResponse } from '../../models/product.model';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../core/services/product.service';

@Component({
  imports: [],
  selector: 'app-product-details',
  styleUrl: './product-details.css',
  templateUrl: './product-details.html',
})
export class ProductDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly productService = inject(ProductService);

  product = signal<ProductResponse | null>(null);
  productId = this.route.snapshot.paramMap.get('id');

  ngOnInit(): void {
    this.productService.getProduct(this.productId!).subscribe((result) => {
      if (result.isSuccess) {
        this.product.set(result.data);
      } else {
        debugger;
        let x = result.errorMessages;
        alert(result.errorMessages);
      }
    });
  }
}
