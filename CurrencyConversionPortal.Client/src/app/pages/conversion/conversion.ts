import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CurrencyService, Currency, ConversionResult } from '../../services/currency.service';

@Component({
  selector: 'app-conversion',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './conversion.html',
  styleUrl: './conversion.scss'
})
export class Conversion implements OnInit {
  conversionForm: FormGroup;
  currencies: Currency[] = [];
  conversionResults: ConversionResult[] = [];
  loading = false;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private currencyService: CurrencyService,
    private router: Router
  ) {
    this.conversionForm = this.fb.group({
      amount: ['', [Validators.required, Validators.min(0.01)]],
      sourceCurrency: ['', Validators.required]
    });
  }

  async ngOnInit() {
    await this.loadCurrencies();
  }

  async loadCurrencies() {
    try {
      this.loading = true;
      this.currencies = await this.currencyService.getCurrencies();
    } catch (error) {
      if (error instanceof Error && error.message === 'AUTHENTICATION_REQUIRED') {
        console.log('Authentication required, redirecting to login...');
        this.router.navigate(['/login']);
        return;
      }
      this.errorMessage = 'Failed to load currencies';
    } finally {
      this.loading = false;
    }
  }

  async onConvert() {
    if (this.conversionForm.invalid) return;

    this.loading = true;
    this.errorMessage = null;
    
    const { amount, sourceCurrency } = this.conversionForm.value;
    
    try {
      const results = await this.currencyService.convertCurrency(amount, sourceCurrency);
      this.conversionResults = results;
    } catch (error) {
      if (error instanceof Error && error.message === 'AUTHENTICATION_REQUIRED') {
        console.log('Authentication required for conversion, redirecting to login...');
        this.router.navigate(['/login']);
        return;
      }
      this.errorMessage = 'Conversion failed. Please try again.';
    } finally {
      this.loading = false;
    }
  }

  get targetCurrencies(): Currency[] {
    const sourceCurrency = this.conversionForm.get('sourceCurrency')?.value;
    return this.currencies.filter(currency => currency.code !== sourceCurrency);
  }
}
