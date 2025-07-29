# Currency Conversion Portal

A full-stack application built with .NET 8 and Angular for currency conversion.

## Quick Start

### Backend (.NET API)
```bash
cd CurrencyConversionPortal.Api
dotnet build
dotnet run
```
API will be available at: `http://localhost:5163`

### Frontend (Angular SPA)
```bash
cd CurrencyConversionPortal.Client
npm install
npm start
```
Frontend will be available at: `http://localhost:4200`

## Production Deployment

### Backend
```bash
# Build for production
dotnet build --configuration Release
# Production environment will automatically use HTTPS-only cookies
```

### Frontend  
```bash
# Build for production (uses HTTPS environment)
npm run build
# This uses environment.prod.ts with HTTPS API URL
```

**Important**: Before production deployment, update the production domain in:
- `CurrencyConversionPortal.Client/src/app/environments/environment.prod.ts`
- `CurrencyConversionPortal.Api/Program.cs` (CORS policy)

## No Certificate Setup Required

The application is configured to work with HTTP in development:
- No HTTPS certificate trust required
- No additional setup steps
- Works directly with `dotnet run` and `npm start`

## Environment-Specific Configuration

### Development
- **API**: HTTP on `localhost:5163`
- **Frontend**: HTTP on `localhost:4200`
- **Cookies**: `SecurePolicy.SameAsRequest` (allows HTTP)
- **SSL**: Disabled for easy development

### Production
- **API**: HTTPS (secure cookies enforced)
- **Frontend**: HTTPS with SSL enabled
- **Cookies**: `SecurePolicy.Always` (requires HTTPS)
- **CORS**: Update `myportaldomain.com` in Program.cs

## Architecture

- **Backend**: .NET 8 Web API with cookie authentication
- **Frontend**: Angular with standalone components
- **Security**: Environment-aware cookie security (HTTP in dev, HTTPS in production)
