export class ProductToInsert {
    id: number
    image: File | null
    name: string
    description: string
    price: number
    stock: number
    categoryName: string

    constructor(image: File | null, name: string, description: string, price: number, stock: number, categoryName: string, id: number)
    {
        this.image = image
        this.name = name
        this.description = description
        this.price = price
        this.stock = stock,
        this.categoryName = categoryName
        this.id = id
    }
}
