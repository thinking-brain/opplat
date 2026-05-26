import { createContext, useContext } from 'react';

export interface AuthUser {
  name: string;
  email: string;
  username: string;
}

export interface AuthContextValue {
  user: AuthUser;
  logout: () => void;
  getToken: () => Promise<string>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

export const useAuth = (): AuthContextValue => {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error('useAuth must be called inside AuthProvider');
  }
  return ctx;
};
