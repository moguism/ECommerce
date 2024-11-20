export class ProductToInsert {
    file: File
    name: string
    description: string
    price: number
    stock: number
    categoryId: number

    constructor(file: File, name: string, description: string, price: number, stock: number, categoryId: number)
    {
        this.file = file
        this.name = name
        this.description = description
        this.price = price
        this.stock = stock,
        this.categoryId = categoryId
    }
}
