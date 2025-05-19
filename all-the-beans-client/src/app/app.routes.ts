import { Routes } from '@angular/router';
import { CoffeeBeanListComponent } from './features/beans/coffee-bean-list/coffee-bean-list.component';
import { CoffeeBeanDetailComponent } from './features/beans/coffee-bean-detail.component';

export const routes: Routes = [
    { path: 'beans', component: CoffeeBeanListComponent },
    { path: 'beans/:id', component: CoffeeBeanDetailComponent },
    { path: '', redirectTo: 'beans', pathMatch: 'full' }
];
