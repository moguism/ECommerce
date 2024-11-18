import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { CreateEthTransactionRequest } from '../models/create-eth-transaction-request';
import { EthereumInfo } from '../models/ethereum-info';
import { CheckTransactionRequest } from '../models/check-transaction-request';
import { Result } from '../models/result';




@Injectable({
  providedIn: 'root'
})
export class BlockchainService {

  constructor(private api: ApiService) { }

  getEthereumInfo(data: CreateEthTransactionRequest): Promise<Result<EthereumInfo>> {
    return this.api.post<EthereumInfo>(`Blockchain/transaction`, data) 
  }

  checkTransaction(data: CheckTransactionRequest): Promise<Result<boolean>> {
    return this.api.post<boolean>(`Blockchain/check`, data)
  }
}
