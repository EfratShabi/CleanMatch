import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import type { CleanerSearchResult } from '@/types/api'

export function CleanerCard({ cleaner }: { cleaner: CleanerSearchResult }) {
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
      </CardContent>
    </Card>
  )
}
