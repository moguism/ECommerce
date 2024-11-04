import { Category } from "./category";
import { Order } from "./order";
import { Review } from "./review";

export interface Product {
    id: number,
    name: string,
    description: string,
    price: number,
    stock: number,
    average: number,
    image: string,
    categoryId: number,
    category: Category,
    reviews: Review,
    orders: Order
}
