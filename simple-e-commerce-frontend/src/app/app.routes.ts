import { Routes } from '@angular/router';
import { Forbidden } from './pages/forbidden/forbidden';
import { Home } from './pages/home/home';


export const routes: Routes = [
	{
		path: '',
		component: Home
	},
	{
		path: 'forbidden',
		component: Forbidden
	},
	{
		path: '**',
		redirectTo: ''
	}
];
