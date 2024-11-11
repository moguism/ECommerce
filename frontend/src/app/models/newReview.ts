export class NewReview {
    Text: string;
    ProductId: number;

    public constructor(text: string, productId: number)
    {
        this.Text = text,
        this.ProductId = productId
    }
}