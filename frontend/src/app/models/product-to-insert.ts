export class ProductToInsert {
    image: File
    name: string
    description: string
    price: number
    stock: number
    categoryId: number

    constructor(image: File, name: string, description: string, price: number, stock: number, categoryId: number)
    {
        this.image = image
        this.name = name
        this.description = description
        this.price = price
        this.stock = stock,
        this.categoryId = categoryId
    }
}
