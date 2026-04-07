import { Component } from '@angular/core';
import { ProductsList } from "../../../shared/products/products-list/products-list";

@Component({
  selector: 'app-customer-product',
  imports: [ProductsList],
  templateUrl: './customer-product.html',
  styleUrl: './customer-product.css',
})
export class CustomerProduct {}
