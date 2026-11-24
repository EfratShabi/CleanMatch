import { useMutation, useQueryClient } from '@tanstack/react-query'
import { Link, useSearchParams } from 'react-router-dom'
import { createBooking } from '@/api/bookings'
import { useAuth } from '@/auth/useAuth'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { getErrorMessage } from '@/lib/errors'
import { addHours, formatDate, hhmm } from '@/lib/time'
import type { CleanerProfile } from '@/types/api'

export function BookingPanel({ cleaner }: { cleaner: CleanerProfile }) {
  const { user } = useAuth()
  const queryClient = useQueryClient()
  const [searchParams] = useSearchParams()

  // The time that was searched arrives in the URL (see CleanerCard).
  const date = searchParams.get('date')
  const start = searchParams.get('start')
  const hours = Number(searchParams.get('hours'))
  const validTime =
    date !== null && /^\d{4}-\d{2}-\d{2}$/.test(date) && start !== null && /^\d{2}:\d{2}$/.test(start) && hours > 0
  const end = validTime ? addHours(start, hours) : null

  const mutation = useMutation({
    mutationFn: () =>
      createBooking({ date: `${date}T00:00:00`, startWork: `${start}:00`, endWork: end!, cleanersId: cleaner.id }),
    onSuccess: () => {
      // The cleaner is no longer free at that time, so cached search results are out of date.
      void queryClient.invalidateQueries({ queryKey: ['search'] })
    },
  })

  if (user?.role !== 'customer') {
    return null
  }

  if (mutation.isSuccess) {
    const booking = mutation.data
    return (
      <Card>
        <CardHeader>
          <CardTitle>ההזמנה נקלטה ✓</CardTitle>
        </CardHeader>
        <CardContent className="space-y-1 text-sm">
          <p>מספר הזמנה: {booking.bookingCode}</p>
          <p>
            {formatDate(booking.date)} · {hhmm(booking.startWork)}–{hhmm(booking.endWork)}
          </p>
          <p>סה״כ לתשלום: {booking.totalPrice} ₪</p>
          <Link to="/" className="inline-block pt-2 underline">לדף הבית</Link>
        </CardContent>
      </Card>
    )
  }

  if (!validTime || end === null) {
    return (
      <Card>
        <CardContent className="space-y-2 pt-6 text-sm">
          <p>כדי להזמין, חפשי קודם לפי תאריך ושעה.</p>
          <Link to="/search" className="underline">לחיפוש</Link>
        </CardContent>
      </Card>
    )
  }

  const price = Math.round(hours * cleaner.pricePerHour * 100) / 100

  return (
    <Card>
      <CardHeader>
        <CardTitle>הזמנה</CardTitle>
      </CardHeader>
      <CardContent className="space-y-3 text-sm">
        <p>
          {formatDate(date)} · {start}–{hhmm(end)} ({hours} שעות)
        </p>
        <p>
          מחיר: <strong>{price} ₪</strong>
        </p>
        {mutation.isError && <p className="text-destructive">{getErrorMessage(mutation.error)}</p>}
        <Button className="w-full" disabled={mutation.isPending} onClick={() => mutation.mutate()}>
          {mutation.isPending ? 'שולחת...' : 'אישור הזמנה'}
        </Button>
      </CardContent>
    </Card>
  )
}
