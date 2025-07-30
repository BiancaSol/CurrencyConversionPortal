import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { ErrorService } from './error.service';

export interface Currency {
  code: string;
  name: string;
}

export interface ApiCurrency {
  code: string;
  description: string;
}

export interface ApiCurrenciesResponse {
  currencies: ApiCurrency[];
}

export interface ApiConversionResponse {
  sourceAmount: number;
  sourceCurrency: string;
  convertedAmounts: { [currencyCode: string]: number };
}

export interface ConversionResult {
  currency: Currency;
  convertedValue: number;
}

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  private apiUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private errorService: ErrorService
  ) {}  async getCurrencies(): Promise<Currency[]> {
    try {
      const data = await firstValueFrom(
        this.http.get<ApiCurrenciesResponse>(`${this.apiUrl}/currencies`, { withCredentials: true })
      );
      
      const currencies: Currency[] = data.currencies.map(apiCurrency => ({
        code: apiCurrency.code,
        name: apiCurrency.description
      }));
      
      return currencies;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        if (this.errorService.isAuthError(error) || 
            (error.status === 405 && error.url && error.url.includes('/auth/login'))) {
          this.errorService.handleAuthError(error);
          throw new Error('AUTH_REQUIRED');
        }
        
        throw new Error(this.errorService.getErrorMessage(error));
      }
      
      throw new Error('Unable to load currencies');
    }
  }  async convertCurrency(amount: number, sourceCurrency: string): Promise<ConversionResult[]> {
    try {
      const data = await firstValueFrom(
        this.http.post<ApiConversionResponse>(`${this.apiUrl}/currencies/convert`, {
          amount,
          sourceCurrency
        }, { withCredentials: true })
      );
      
      const allCurrencies = await this.getCurrencies();
    
      const results: ConversionResult[] = Object.entries(data.convertedAmounts).map(([currencyCode, convertedValue]) => {
        const currency = allCurrencies.find(c => c.code === currencyCode);
        return {
          currency: currency || { code: currencyCode, name: currencyCode },
          convertedValue: convertedValue
        };
      });
      
      return results;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        if (this.errorService.isAuthError(error) || 
            (error.status === 405 && error.url && error.url.includes('/auth/login'))) {
          this.errorService.handleAuthError(error);
          throw new Error('AUTH_REQUIRED');
        }
        throw new Error(this.errorService.getErrorMessage(error));
      }
      
      throw new Error('Unable to convert currency');
    }
  }
}
