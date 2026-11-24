import { apiFetch } from '@/lib/api'
import type { CleanerProfile, Feedback } from '@/types/api'

export function getCleaner(id: number, signal?: AbortSignal) {
  return apiFetch<CleanerProfile>(`/api/cleaners/${id}`, { signal })
}

export function getCleanerFeedbacks(cleanerId: number, signal?: AbortSignal) {
  return apiFetch<Feedback[]>(`/api/feedbacks?cleanerId=${cleanerId}`, { signal })
}
