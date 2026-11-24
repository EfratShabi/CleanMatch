import type { ReactNode } from 'react'
import { Navigate } from 'react-router-dom'
import { useAuth } from '@/auth/useAuth'
import type { Role } from '@/types/api'

type Props = {
  children: ReactNode
  // when given, only these roles may see the page
  roles?: Role[]
}

export function RequireAuth({ children, roles }: Props) {
  const { user, isLoading } = useAuth()

  if (isLoading) {
    return <p className="p-6">טוענת...</p>
  }
  if (user === null) {
    return <Navigate to="/login" replace />
  }
  if (roles && !roles.includes(user.role)) {
    return <Navigate to="/" replace />
  }
  return <>{children}</>
}
