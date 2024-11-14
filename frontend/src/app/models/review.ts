import { User } from "./user";

export interface Review {
    id: number,
    text: string,
    score: number,
    userId: number,
    productId: number,
    dateTime: Date,
    user: User
}
