import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { CreateEthTransactionRequest } from '../models/create-eth-transaction-request';
import { EthereumInfo } from '../models/ethereum-info';
import { CheckTransactionRequest } from '../models/check-transaction-request';
import { Result } from '../models/result';
import { Order } from '../models/order';




@Injectable({
  providedIn: 'root'
})
export class BlockchainService {

  constructor(private api: ApiService) { }

  getEthereumInfo(data: CreateEthTransactionRequest): Promise<Result<EthereumInfo>> {
    return this.api.post<EthereumInfo>(`Blockchain/transaction`, data) 
  }

  checkTransaction(data: CheckTransactionRequest): Promise<Result<Order>> {
    return this.api.post<Order>(`Blockchain/check`, data)
  }
}
