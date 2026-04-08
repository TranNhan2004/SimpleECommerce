import { Component, signal } from '@angular/core';
import { ProductResponse } from '../../../core/models/product';
import { ProductCard } from '../product-card/product-card';

@Component({
  selector: 'app-products-list',
  imports: [ProductCard],
  templateUrl: './products-list.html',
  styleUrl: './products-list.css',
})
export class ProductsList {
  protected readonly products = signal<ProductResponse[]>([
    createProduct({
      id: 'product-1',
      name: 'Aurora Wireless Headphones',
      description: 'Over-ear headphones with balanced sound and long battery life.',
      currentPrice: { amount: 129.99, currency: 'USD' },
      totalInStock: 24,
      status: 'Active',
      categoryId: 'category-audio',
      sellerId: 'seller-01',
      rating: 4.8,
      baseLabel: 'AURORA',
      imageStart: '#dbeafe',
      imageEnd: '#ffe4e6'
    }),
    createProduct({
      id: 'product-2',
      name: 'Metro Runner Shoes',
      description: 'Lightweight everyday shoes built for comfort and movement.',
      currentPrice: { amount: 89.5, currency: 'USD' },
      totalInStock: 16,
      status: 'Active',
      categoryId: 'category-fashion',
      sellerId: 'seller-02',
      rating: 4.5,
      baseLabel: 'METRO',
      imageStart: '#ede9fe',
      imageEnd: '#ffe4e6'
    }),
    createProduct({
      id: 'product-3',
      name: 'Nova Smart Watch',
      description: 'Slim smartwatch with health tracking and bright display.',
      currentPrice: { amount: 159, currency: 'USD' },
      totalInStock: 8,
      status: 'Draft',
      categoryId: 'category-accessories',
      sellerId: 'seller-03',
      rating: 4.9,
      baseLabel: 'NOVA',
      imageStart: '#dcfce7',
      imageEnd: '#e0f2fe'
    }),
    createProduct({
      id: 'product-4',
      name: 'Studio Desk Lamp',
      description: 'Minimal desk lamp with warm light and a soft matte finish.',
      currentPrice: { amount: 45, currency: 'USD' },
      totalInStock: 31,
      status: 'Hidden',
      categoryId: 'category-home',
      sellerId: 'seller-04',
      rating: 4.2,
      baseLabel: 'STUDIO',
      imageStart: '#fef3c7',
      imageEnd: '#ffe4e6'
    })
  ]);
}

function createProduct(input: {
  id: string;
  name: string;
  description: string;
  currentPrice: ProductResponse['currentPrice'];
  totalInStock: number;
  status: ProductResponse['status'];
  categoryId: string;
  sellerId: string;
  rating: number;
  baseLabel: string;
  imageStart: string;
  imageEnd: string;
}): ProductResponse {
  const now = new Date().toISOString();
  const imageUrl = createPlaceholderImage(input.baseLabel, input.imageStart, input.imageEnd);

  return {
    id: input.id,
    name: input.name,
    description: input.description,
    currentPrice: input.currentPrice,
    totalInStock: input.totalInStock,
    status: input.status,
    categoryId: input.categoryId,
    sellerId: input.sellerId,
    productImages: [
      {
        id: `${input.id}-image-1`,
        imageUrl,
        displayOrder: 1,
        isDisplayed: true,
        description: `${input.name} preview`,
        createdAt: now
      }
    ],
    productPrices: [
      {
        id: `${input.id}-price-1`,
        money: input.currentPrice,
        effectiveFrom: now,
        createdAt: now
      },
      {
        id: `${input.id}-price-2`,
        money: {
          amount: Number((input.currentPrice.amount * 0.92).toFixed(2)),
          currency: input.currentPrice.currency
        },
        effectiveFrom: new Date(Date.now() - 1000 * 60 * 60 * 24 * 14).toISOString(),
        createdAt: now
      }
    ],
    createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24 * 30).toISOString(),
    updatedAt: now,
    rating: input.rating
  };
}

function createPlaceholderImage(label: string, imageStart: string, imageEnd: string): string {
  const svg = `
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 800 800" role="img" aria-label="${label}">
      <defs>
        <linearGradient id="bg" x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" stop-color="${imageStart}" />
          <stop offset="100%" stop-color="${imageEnd}" />
        </linearGradient>
      </defs>
      <rect width="800" height="800" rx="64" fill="url(#bg)" />
      <circle cx="400" cy="290" r="126" fill="#ffffff" fill-opacity="0.45" />
      <text x="400" y="458" text-anchor="middle" font-family="Arial, sans-serif" font-size="56" font-weight="700" fill="#0f172a">${label}</text>
    </svg>`;

  return `data:image/svg+xml;charset=UTF-8,${encodeURIComponent(svg)}`;
}
