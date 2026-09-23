import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { ProductDetails } from './shared/product-details/product-details';

export const routes: Routes = [
  {
    path: '',
    component: Home,
  },
  {
    path: 'product-details/:id',
    component: ProductDetails,
  },
];
