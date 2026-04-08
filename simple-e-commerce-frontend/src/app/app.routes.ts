import { Routes } from '@angular/router';
import { Forbidden } from './pages/forbidden/forbidden';
import { Home } from './pages/home/home';
import { NotFound } from './pages/not-found/not-found';
import { notAuthGuard } from './core/guards/not-auth.guard';
import { authGuard } from './core/guards/auth.guard';
import { Role } from './core/enums/role';


export const routes: Routes = [
	{
		path: '',
		pathMatch: 'full',
    canActivate: [notAuthGuard],
		component: Home
	},
	{
		path: 'forbidden',
		component: Forbidden
	},
	{
		path: 'not-found',
		component: NotFound
	},
  {
    path: 'customer',
    canActivate: [authGuard],
    data: { roles: [Role.Customer] },
    loadChildren: () => import('./pages/customer/customer.routes').then(m => m.customerRoutes)
  },
	{
		path: '**',
		component: NotFound
	}
];
