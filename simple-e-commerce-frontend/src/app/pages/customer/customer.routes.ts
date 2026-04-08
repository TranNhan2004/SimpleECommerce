import { Routes } from "@angular/router";
import { CustomerProduct } from "./customer-product/customer-product";

export const customerRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'products'
  },
  {
    path: 'products',
    component: CustomerProduct
  }
]
