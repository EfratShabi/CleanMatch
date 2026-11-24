import { useQuery } from '@tanstack/react-query'
import { Link, useParams } from 'react-router-dom'
import { getCleaner, getCleanerFeedbacks } from '@/api/cleaners'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { ApiError } from '@/lib/api'
import { getErrorMessage } from '@/lib/errors'
import { AvailabilityList } from '@/pages/profile/AvailabilityList'
import { ReviewList } from '@/pages/profile/ReviewList'

export default function CleanerProfilePage() {
  const { id } = useParams()
  const cleanerId = Number(id)

  // Two independent queries: React Query runs them in parallel.
  const cleanerQuery = useQuery({
    queryKey: ['cleaner', cleanerId],
    queryFn: ({ signal }) => getCleaner(cleanerId, signal),
    retry: false,
  })
  const reviewsQuery = useQuery({
    queryKey: ['feedbacks', cleanerId],
    queryFn: ({ signal }) => getCleanerFeedbacks(cleanerId, signal),
    retry: false,
  })

  if (cleanerQuery.isLoading) {
    return <p className="p-6">טוענת...</p>
  }
  if (cleanerQuery.isError) {
    const notFound = cleanerQuery.error instanceof ApiError && cleanerQuery.error.status === 404
    return (
      <main className="mx-auto max-w-xl space-y-3 p-6">
        <p className="text-destructive">{notFound ? 'המנקה לא נמצאה.' : getErrorMessage(cleanerQuery.error)}</p>
        <Link to="/search" className="text-sm underline">חזרה לחיפוש</Link>
      </main>
    )
  }

  const cleaner = cleanerQuery.data!
  const rating =
    cleaner.averageRating === null
      ? 'אין דירוג עדיין'
      : `${cleaner.averageRating.toFixed(1)} ★ (${cleaner.reviewCount} חוות דעת)`

  return (
    <main className="mx-auto max-w-xl space-y-4 p-6">
      <Card>
        <CardHeader>
          <CardTitle>
            {cleaner.firstName} {cleaner.lastName}
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-1 text-sm">
          <p>{cleaner.city} · עד {cleaner.upToKm} ק״מ</p>
          <p>{rating}</p>
          <p>{cleaner.pricePerHour} ₪ לשעה</p>
          <p>
            {cleaner.hebrewSpeaker ? 'דוברת עברית' : 'לא דוברת עברית'}
            {cleaner.bornIsrael ? ' · נולדה בישראל' : ''}
          </p>
          {cleaner.descriptionOnMe && <p className="pt-2">{cleaner.descriptionOnMe}</p>}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>שעות עבודה</CardTitle>
        </CardHeader>
        <CardContent>
          <AvailabilityList slots={cleaner.arrAvailability} />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>חוות דעת</CardTitle>
        </CardHeader>
        <CardContent>
          {reviewsQuery.isLoading && <p className="text-sm">טוענת...</p>}
          {reviewsQuery.isError && (
            <p className="text-sm text-destructive">{getErrorMessage(reviewsQuery.error)}</p>
          )}
          {reviewsQuery.data && <ReviewList reviews={reviewsQuery.data} />}
        </CardContent>
      </Card>

      <Link to="/search" className="text-sm underline">חזרה לחיפוש</Link>
    </main>
  )
}
