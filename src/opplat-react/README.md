# Opplat React Application

Modern React 18 application built with Vite, TypeScript, and Material-UI.

## Tech Stack

- **React 18** - Modern UI library
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool and dev server
- **Material-UI (MUI) v5** - Component library
- **React Router v6** - Client-side routing
- **Axios** - HTTP client with JWT interceptors

## Features

- ✅ JWT Authentication with token storage
- ✅ Protected routes with automatic redirect
- ✅ Responsive layout with sidebar navigation
- ✅ Product management (CRUD operations)
- ✅ Point of Sale interface
- ✅ User management with roles
- ✅ Modern Material Design UI

## Project Structure

```
src/
├── api/              # API client and service modules
│   ├── axiosClient.ts       # Configured Axios instance with interceptors
│   ├── auth.api.ts          # Authentication endpoints
│   ├── products.api.ts      # Product management endpoints
│   ├── sales.api.ts         # Sales endpoints
│   └── users.api.ts         # User management endpoints
├── auth/             # Authentication logic
│   ├── AuthContext.tsx      # React Context for auth state
│   └── ProtectedRoute.tsx   # Route guard component
├── components/       # Reusable components
│   ├── Layout.tsx           # Main layout with navbar and sidebar
│   └── LoadingSpinner.tsx   # Loading indicator
├── pages/            # Page components
│   ├── LoginPage.tsx        # Authentication page
│   ├── HomePage.tsx         # Dashboard
│   ├── ProductsPage.tsx     # Product management
│   ├── SellPage.tsx         # Point of Sale
│   └── UsersPage.tsx        # User management
├── types/            # TypeScript type definitions
│   └── index.ts             # Shared types
├── App.tsx           # Main app component with routes
└── main.tsx          # Application entry point
```

## Setup Instructions

### 1. Install Dependencies

```bash
cd src/opplat-react
npm install
```

### 2. Configure Environment

Copy `.env.example` to `.env` and configure the API URL:

```bash
cp .env.example .env
```

Edit `.env`:
```
VITE_API_URL=http://localhost:5000
```

### 3. Run Development Server

```bash
npm run dev
```

The application will open at `http://localhost:3000`

### 4. Build for Production

```bash
npm run build
```

The build output will be in the `dist/` directory.

## API Integration

The application connects to the ASP.NET Core backend at the configured `VITE_API_URL`.

### Authentication Flow

1. User logs in at `/login` with username and password
2. JWT token is received from `/auth/Account/Login`
3. Token is stored in localStorage as `opplat_token`
4. Token is automatically added to all API requests via Axios interceptor
5. On 401 response, user is redirected to login

### API Endpoints Used

- **Auth**: `/auth/Account/*`
  - POST `/Login` - Authenticate user
  - GET `/profile/{name}` - Get user profile
  - GET `/user-list` - List all users
  - POST `/add-user` - Create user
  - POST `/edit-user` - Update user
  - GET `/cambiar-estado` - Toggle user active status
  - POST `/cambiar-roles` - Update user roles

- **Products**: `/sales/Products`
  - GET `/` - List products
  - POST `/` - Create product
  - PUT `/` - Update product
  - DELETE `/?id={id}` - Delete product

- **Sales**: `/Sales`
  - GET `/` - List sales
  - POST `/` - Create sale

## Token Storage

- **Key**: `opplat_token` in localStorage
- **Format**: `Bearer {jwt-token}`
- **User Data**: `opplat_user` in localStorage (JSON)

## Development Notes

- Uses Vite for fast HMR (Hot Module Replacement)
- TypeScript strict mode enabled
- MUI theming configured in `main.tsx`
- All API calls go through Axios client with interceptors
- Protected routes automatically redirect to `/login` if not authenticated

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Type check with TypeScript

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Migration from Vue 2

This React application maintains functional parity with the existing Vue 2 application:

- Same routes: `/login`, `/home`, `/products`, `/sell`, `/users`
- Same API endpoints and data structures
- Same JWT authentication mechanism
- Compatible token storage (sessionStorage in Vue → localStorage in React)
