import { apiFetch } from '@/lib/api'
import type { Booking, BookingCreate } from '@/types/api'

export function createBooking(data: BookingCreate) {
  return apiFetch<Booking>('/api/bookings', { method: 'POST', body: data })
}
