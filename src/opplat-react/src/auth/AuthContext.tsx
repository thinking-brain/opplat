import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../api/auth.api';
import { User, LoginRequest } from '../types';

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  logout: () => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const storedToken = localStorage.getItem('opplat_token');
    const storedUser = localStorage.getItem('opplat_user');
    
    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
    }
    setLoading(false);
  }, []);

  const login = async (credentials: LoginRequest) => {
    try {
      const response = await authApi.login(credentials);
      const tokenWithBearer = `Bearer ${response.token}`;
      
      const payload = JSON.parse(atob(response.token.split('.')[1]));
      const username = payload.unique_name;
      const roles = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || [];
      
      const userData: User = {
        userId: response.userId,
        username: username,
        name: username,
        lastName: '',
        email: payload.email || '',
        active: true,
        roles: Array.isArray(roles) ? roles : [roles],
      };

      localStorage.setItem('opplat_token', tokenWithBearer);
      localStorage.setItem('opplat_user', JSON.stringify(userData));
      
      setToken(tokenWithBearer);
      setUser(userData);
      
      navigate('/');
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  };

  const logout = () => {
    authApi.logout();
    setToken(null);
    setUser(null);
    navigate('/login');
  };

  const value = {
    user,
    token,
    isAuthenticated: !!token,
    login,
    logout,
    loading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
