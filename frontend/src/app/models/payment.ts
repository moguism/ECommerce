export interface Payment {
    id: number,
    createdAt: Date,
    total: number,
    isDone: number,
    orderId: number,
    paymentTypeId: number
}
