import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductResponse } from '../../models/product.model';
import { Pagination } from '../../models/pagination.model';
import { Result } from '../../models/result.model';

@Service()
export class ProductService {
  private readonly http = inject(HttpClient);

  private readonly apiUrlBase = 'http://localhost/Catalog/api/v1';

  getProducts(): Observable<Result<Pagination<ProductResponse>>> {
    return this.http.get<Result<Pagination<ProductResponse>>>(this.apiUrlBase + '/products');
  }

  getProduct(id: string): Observable<Result<ProductResponse>> {
    return this.http.get<Result<ProductResponse>>(`${this.apiUrlBase + '/products'}/${id}`);
  }

  searchProducts(search: string): Observable<Result<Pagination<ProductResponse>>> {
    return this.http.get<Result<Pagination<ProductResponse>>>(
      `${this.apiUrlBase + '/products'}?ProductName=${encodeURIComponent(search)}`,
    );
  }
}
