import { Product } from "./product";

export interface PagedProducts {
    products : Product[] | null;
    totalProducts : number;
}
