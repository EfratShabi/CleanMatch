import { apiFetch } from '@/lib/api'
import type { CleanerSignup, CustomerSignup } from '@/types/api'

// Signup is public, so no token is sent (auth: false).
export function signupCustomer(data: CustomerSignup) {
  return apiFetch<unknown>('/api/customers', { method: 'POST', body: data, auth: false })
}

export function signupCleaner(data: CleanerSignup) {
  return apiFetch<unknown>('/api/cleaners', { method: 'POST', body: data, auth: false })
}
