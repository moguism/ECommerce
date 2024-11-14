export class NewReview {
    Text: string;
    ProductId: number;
    DateTime: string;

    public constructor(text: string, productId: number, dateTime: string)
    {
        this.Text = text,
        this.ProductId = productId,
        this.DateTime = dateTime
    }
}