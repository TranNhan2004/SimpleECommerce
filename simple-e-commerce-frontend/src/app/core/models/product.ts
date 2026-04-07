export type ProductStatusResponse = 'Draft' | 'Active' | 'Hidden';

export interface MoneyResponse {
	amount: number;
	currency: string;
}

export interface ProductImageResponse {
	id: string;
	imageUrl: string;
	displayOrder: number;
	isDisplayed: boolean;
	description: string | null;
	createdAt: string;
}

export interface ProductPriceResponse {
	id: string;
	money: MoneyResponse;
	effectiveFrom: string;
	createdAt: string;
}

export interface ProductResponse {
	id: string;
	name: string;
	description: string;
	currentPrice: MoneyResponse;
	totalInStock: number;
	status: ProductStatusResponse;
	categoryId: string;
	sellerId: string;
	productImages: ProductImageResponse[];
	productPrices: ProductPriceResponse[];
	createdAt: string;
	updatedAt: string | null;
	rating?: number;
}
