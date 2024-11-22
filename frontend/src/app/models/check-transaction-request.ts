export interface CheckTransactionRequest {
    networkUrl: string;
    hash: string;
    to: string;
    from: string;
    value: string;
}
