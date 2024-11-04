import { Product } from "./product";

export interface Order {
    id: number,
    createdAt: Date,
    isReserved: boolean,
    products: Product[]
}
