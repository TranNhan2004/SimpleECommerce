import { Component, computed, input } from '@angular/core';
import { MoneyResponse, ProductResponse } from '../../../core/models/product';

@Component({
  selector: 'app-product-card',
  imports: [],
  templateUrl: './product-card.html',
  styleUrl: './product-card.css',
})
export class ProductCard {
  readonly product = input.required<ProductResponse>();
  readonly primaryImage = computed(() => {
    const product = this.product();
    return product.productImages.find((image) => image.isDisplayed) ?? product.productImages[0] ?? null;
  });

  protected formatMoney(money: MoneyResponse): string {
    try {
      return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: money.currency
      }).format(money.amount);
    } catch {
      return `${money.amount.toLocaleString()} ${money.currency}`;
    }
  }
}
