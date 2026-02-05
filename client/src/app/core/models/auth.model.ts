export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  username: string;
  phoneNumber: string;
  firstName: string;
  lastName: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserAuthResponse;
}

export interface UserAuthResponse {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: string | number; // Backend sends numeric enum (0=AppUser, 1=Admin)
}

export interface RefreshRequest {
  refreshToken: string;
}
