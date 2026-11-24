import { Link } from 'react-router-dom'
import { buttonVariants } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import type { CleanerSearchResult, SearchParams } from '@/types/api'

type Props = { cleaner: CleanerSearchResult; search: SearchParams }

export function CleanerCard({ cleaner, search }: Props) {
  // The chosen time travels in the URL, so the profile (and later the booking) knows what was searched.
  const profileUrl = `/cleaners/${cleaner.id}?${new URLSearchParams({
    date: search.date,
    start: search.start,
    hours: String(search.hours),
  })}`

  const rating =
    cleaner.averageRating === null
      ? 'אין דירוג עדיין'
      : `${cleaner.averageRating.toFixed(1)} ★ (${cleaner.reviewCount} חוות דעת)`

  return (
    <Card>
      <CardHeader>
        <CardTitle>
          {cleaner.firstName} {cleaner.lastName}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-1 text-sm">
        <p>{cleaner.city} · {cleaner.distanceKm.toFixed(1)} ק״מ ממך</p>
        <p>{rating}</p>
        <p>
          {cleaner.pricePerHour} ₪ לשעה · סה״כ <strong>{cleaner.totalPrice} ₪</strong>
        </p>
        {cleaner.descriptionOnMe && <p className="text-muted-foreground">{cleaner.descriptionOnMe}</p>}
        <Link to={profileUrl} className={buttonVariants({ variant: 'outline', size: 'sm' })}>
          לפרופיל המלא
        </Link>
      </CardContent>
    </Card>
  )
}
