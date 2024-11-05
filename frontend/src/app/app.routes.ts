import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { UserComponent } from './pages/user/user.component';
//import { ProductViewComponent } from './pages/product-view/ProductViewComponent';
import { AboutUsComponent } from './pages/about-us/about-us.component';
import { ProductViewComponent } from './pages/product-view/product-view.component';
import { ProductListComponent } from './pages/product-list/product-list.component';
import { ShoppingCartComponent } from './pages/shopping-cart/shopping-cart.component';
import { PruebaShoppingCartComponent } from './pages/prueba-shopping-cart/prueba-shopping-cart.component';


export const routes: Routes = [
    {path: "", component: HomeComponent},
    {path: "login", component: LoginComponent},
    {path: "user", component: UserComponent},
    {path: "about-us", component: AboutUsComponent},
    {path: "product-view/:id", component: ProductViewComponent},
    {path: "product-list/:category", component: ProductListComponent},
    {path: "shopping-cart", component: ShoppingCartComponent},
    {path: "prueba-shopping-cart", component: PruebaShoppingCartComponent}
];
