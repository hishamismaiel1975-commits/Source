import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductResponse } from '../../models/product.model';
import { Pagination } from '../../models/pagination.model';
import { Result } from '../../models/result.model';
import { environment } from '../../../environments/environment';

@Service()
export class ProductService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = environment.apiBaseUrl + environment.apiCatalogUrl;

  getProducts(): Observable<Result<Pagination<ProductResponse>>> {
    return this.http.get<Result<Pagination<ProductResponse>>>(this.apiUrl + '/products');
  }

  getProduct(id: string): Observable<Result<ProductResponse>> {
    return this.http.get<Result<ProductResponse>>(`${this.apiUrl + '/products'}/${id}`);
  }

  searchProducts(search: string): Observable<Result<Pagination<ProductResponse>>> {
    return this.http.get<Result<Pagination<ProductResponse>>>(
      `${this.apiUrl + '/products'}?ProductName=${encodeURIComponent(search)}`,
    );
  }
}
