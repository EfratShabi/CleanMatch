import { ApiError } from '@/lib/api'

export function getErrorMessage(error: unknown, fallback = 'משהו השתבש. נסי שוב.'): string {
  return error instanceof ApiError ? error.message : fallback
}
