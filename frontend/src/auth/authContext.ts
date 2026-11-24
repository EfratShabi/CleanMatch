import { createContext } from 'react'
import type { Me } from '@/types/api'

export type AuthState = {
  user: Me | null
  isLoading: boolean
  login: (userName: string, password: string) => Promise<void>
  logout: () => void
}

export const AuthContext = createContext<AuthState | null>(null)
