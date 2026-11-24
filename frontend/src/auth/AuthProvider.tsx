import type { ReactNode } from 'react'
import { useQuery, useQueryClient } from '@tanstack/react-query'
import { AuthContext } from '@/auth/authContext'
import { apiFetch } from '@/lib/api'
import { clearToken, getToken, setToken } from '@/lib/token'
import type { LoginResponse, Me } from '@/types/api'

export function AuthProvider({ children }: { children: ReactNode }) {
  const queryClient = useQueryClient()

  // Ask the server who the token belongs to; with no token there is nothing to ask.
  const { data: user, isLoading } = useQuery({
    queryKey: ['me'],
    queryFn: () => apiFetch<Me>('/api/auth/me'),
    enabled: getToken() !== null,
    retry: false,
  })

  async function login(userName: string, password: string) {
    const { token } = await apiFetch<LoginResponse>('/api/auth/login', {
      method: 'POST',
      body: { userName, password },
      auth: false,
    })
    setToken(token)
    await queryClient.invalidateQueries({ queryKey: ['me'] })
  }

  function logout() {
    clearToken()
    queryClient.clear()
    window.location.assign('/login')
  }

  return (
    <AuthContext.Provider value={{ user: user ?? null, isLoading, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}
