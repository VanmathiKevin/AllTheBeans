import { Routes } from '@angular/router';
import { CoffeeBeanListComponent } from './features/beans/coffee-bean-list/coffee-bean-list.component';
import { CoffeeBeanDetailComponent } from './features/beans/coffee-bean-detail.component';
import { BeanOfTheDayComponent } from './features/beans/bean-of-the-day.component';
import { OrderFormComponent } from './features/orders/order-form.component';
import { HomeComponent } from './features/home/home.component';
import { LoginComponent } from './features/login/login.component';

export const routes: Routes = [
    { path: 'beans', component: CoffeeBeanListComponent },
    { path: 'beans/:id', component: CoffeeBeanDetailComponent },
    { path: 'bean-of-the-day', component: BeanOfTheDayComponent },
    { path: 'order', component: OrderFormComponent },
    { path: 'login', component: LoginComponent },
    { path: '', component: HomeComponent }
];
