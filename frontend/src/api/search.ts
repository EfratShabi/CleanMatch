import { apiFetch } from '@/lib/api'
import type { CleanerSearchResult, SearchParams } from '@/types/api'

export function searchCleaners(params: SearchParams, signal?: AbortSignal) {
  const query = new URLSearchParams({
    date: params.date,
    start: params.start,
    hours: String(params.hours),
  })
  if (params.maxPrice !== null) {
    query.set('maxPrice', String(params.maxPrice))
  }
  return apiFetch<CleanerSearchResult[]>(`/api/cleaners/search?${query}`, { signal })
}
