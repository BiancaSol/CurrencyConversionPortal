import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';

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

  constructor(private http: HttpClient) {}  async getCurrencies(): Promise<Currency[]> {
    try {
      console.log('Fetching currencies from:', `${this.apiUrl}/currencies`);
      const data = await firstValueFrom(
        this.http.get<ApiCurrenciesResponse>(`${this.apiUrl}/currencies`, { withCredentials: true })
      );
      console.log('Successfully fetched currencies from API:', data);
      
      // Map the API response format to our internal format
      const currencies: Currency[] = data.currencies.map(apiCurrency => ({
        code: apiCurrency.code,
        name: apiCurrency.description
      }));
      
      return currencies;
    } catch (error) {
      console.error('Error fetching currencies from API:', error);
      
      if (error instanceof HttpErrorResponse) {
        console.error('HTTP Error Details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url
        });
        
        // If we get a 302 or 401, it means authentication is required
        if (error.status === 302 || error.status === 401) {
          console.log('🔒 Authentication required for getCurrencies - throwing AUTHENTICATION_REQUIRED error');
          throw new Error('AUTHENTICATION_REQUIRED');
        }
      }

      return [];
    }
  }  async convertCurrency(amount: number, sourceCurrency: string): Promise<ConversionResult[]> {
    try {
      console.log('Converting currency via API:', { amount, sourceCurrency });
      const data = await firstValueFrom(
        this.http.post<ApiConversionResponse>(`${this.apiUrl}/currencies/convert`, {
          amount,
          sourceCurrency
        }, { withCredentials: true })
      );
      console.log('Successfully converted currency via API:', data);
      
      // Get all currencies to map the converted amounts
      const allCurrencies = await this.getCurrencies();
      
      // Convert the API response format to our internal format
      const results: ConversionResult[] = Object.entries(data.convertedAmounts).map(([currencyCode, convertedValue]) => {
        const currency = allCurrencies.find(c => c.code === currencyCode);
        return {
          currency: currency || { code: currencyCode, name: currencyCode },
          convertedValue: convertedValue
        };
      });
      
      return results;
    } catch (error) {
      console.error('Error converting currency via API:', error);
      
      if (error instanceof HttpErrorResponse) {
        console.error('HTTP Error Details:', {
          status: error.status,
          statusText: error.statusText,
          message: error.message,
          url: error.url
        });
        
        if (error.status === 302 || error.status === 401) {
          throw new Error('AUTHENTICATION_REQUIRED');
        }
      }
      
      console.log('Falling back to empty conversion results due to API error');
      return [];
    }
  }
}
